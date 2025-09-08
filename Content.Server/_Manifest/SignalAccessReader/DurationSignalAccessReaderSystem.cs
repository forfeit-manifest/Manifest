using Content.Server.DeviceLinking.Systems;
using Content.Shared.MNET.CardReader;
using Robust.Server.GameObjects;

namespace Content.Server.MNET.CardReader;

public sealed class DurationSignalAccessReaderSystem : SharedDurationSignalAccessReaderSystem
{
    [Dependency] private readonly DeviceLinkSystem _deviceLinkSystem = default!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;

    public override void ReaderFailed(Entity<DurationSignalAccessReaderComponent> reader, EntityUid user)
    {
        base.ReaderFailed(reader, user);

        var (uid, component) = reader;
        _deviceLinkSystem.SendSignal(uid, component.FailurePort, true);
    }

    public override void ReaderSuccess(Entity<DurationSignalAccessReaderComponent> reader, EntityUid user)
    {
        base.ReaderSuccess(reader, user);

        var (uid, component) = reader;
        _deviceLinkSystem.SendSignal(uid, component.SuccessPort, true);
    }
}