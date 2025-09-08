using Content.Shared.MNET.CardReader;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;

namespace Content.Client.MNET.CardReader;

// This is kinda jank and i don't like it but whatever.
public sealed class DurationSignalAccessReaderSystem : SharedDurationSignalAccessReaderSystem
{
    [Dependency] private readonly AnimationPlayerSystem _animationSystem = default!;
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DurationSignalAccessReaderComponent, AppearanceChangeEvent>(OnReaderAppearanceUpdated);
    }

    private static readonly Animation ReaderFailAnimation = new()
    {
        Length = TimeSpan.FromSeconds(0.5),
        AnimationTracks =
        {
            new AnimationTrackSpriteFlick
            {
                LayerKey = DurationSignalAccessReaderLayers.Fail,
                KeyFrames =
                {
                    new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("red_unlit"), default)
                }
            }
        }
    };

    private static readonly Animation ReaderSuccessAnimation = new()
    {
        Length = TimeSpan.FromSeconds(0.5),
        AnimationTracks =
        {
            new AnimationTrackSpriteFlick
            {
                LayerKey = DurationSignalAccessReaderLayers.Success,
                KeyFrames =
                {
                    new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("green_unlit"), default)
                }
            }
        }
    };

    private const string ReaderFailAnimationKey = "fail";
    private const string ReaderSuccessAnimationKey = "success";

    private void OnReaderAppearanceUpdated(Entity<DurationSignalAccessReaderComponent> reader, ref AppearanceChangeEvent args)
    {
        UpdateReaderAppearance(reader, args.Sprite, args.Component);
    }

    private void UpdateReaderAppearance(Entity<DurationSignalAccessReaderComponent> reader, SpriteComponent? spriteComponent = null, AppearanceComponent? appearanceComponent = null)
    {
        if (spriteComponent == null && !TryComp(reader, out spriteComponent))
            return;

        AppearanceSystem.TryGetData<DurationSignalAccessReaderState?>(reader.Owner, DurationSignalAccessReaderVisuals.State, out var state, appearanceComponent);

        // If this transitions from fail to success (and vice versa) it will have both layers visible until it goes back to something else. But I cant be bothered to fix that.\
        var spriteEntity = (reader.Owner, spriteComponent);
        switch (state)
        {
            case DurationSignalAccessReaderState.Fail:
                _spriteSystem.LayerSetVisible(spriteEntity, DurationSignalAccessReaderLayers.Fail, true);

                if (!_animationSystem.HasRunningAnimation(reader.Owner, ReaderFailAnimationKey))
                    _animationSystem.Play(reader.Owner, ReaderFailAnimation, ReaderFailAnimationKey);

                break;
            case DurationSignalAccessReaderState.Success:
                _spriteSystem.LayerSetVisible(spriteEntity, DurationSignalAccessReaderLayers.Success, true);

                if (!_animationSystem.HasRunningAnimation(reader.Owner, ReaderSuccessAnimationKey))
                    _animationSystem.Play(reader.Owner, ReaderSuccessAnimation, ReaderSuccessAnimationKey);

                break;
            default:
                _spriteSystem.LayerSetVisible(spriteEntity, DurationSignalAccessReaderLayers.Fail, false);
                _spriteSystem.LayerSetVisible(spriteEntity, DurationSignalAccessReaderLayers.Success, false);
                break;
        }
    }
}

public enum DurationSignalAccessReaderLayers : byte
{
    Base = 0,
    Fail = 1,
    Success = 2,
}