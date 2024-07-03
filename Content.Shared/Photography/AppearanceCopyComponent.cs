using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Photography;

[RegisterComponent, NetworkedComponent]
public sealed partial class AppearanceCopyComponent : Component
{
    public string PrototypeId;
}

[Serializable, NetSerializable]
public enum AppearanceCopyVisuals
{
    Prototype
}

[Serializable, NetSerializable]
public sealed class AppearanceCopyComponentState(string prototypeId) : ComponentState
{
    public string PrototypeId = prototypeId;
}
