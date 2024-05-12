using Content.Shared.DeviceNetwork;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Photography;

[RegisterComponent]
[Access(typeof(PhotoSystem))]
public sealed partial class PhotoComponent : Component
{
    // List of active viewers. This is for bookkeeping purposes,
    // so that when a camera shuts down, any entity viewing it
    // will immediately have their subscription revoked.
    [ViewVariables]
    public HashSet<EntityUid> ActiveViewers { get; } = new();

    [ViewVariables]
    public EntityUid PhotoEntity = new();
}
