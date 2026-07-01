using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events
{
    /// <summary>
    /// Provides the full list of players, only intended for admins.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class FullPlayerListEvent : EntityEventArgs
    {
        public List<PlayerInfo> PlayersInfo = new();
    }
}
