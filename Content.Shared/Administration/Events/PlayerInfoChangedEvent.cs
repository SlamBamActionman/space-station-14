using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events;

/// <summary>
/// Updates the info for a specific player in an admin's player list.
/// </summary>
[NetSerializable, Serializable]
public sealed class PlayerInfoChangedEvent : EntityEventArgs
{
    public PlayerInfo? PlayerInfo;
}
