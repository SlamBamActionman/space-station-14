using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.BanList;

[Serializable, NetSerializable]
public sealed class BanListEuiState : EuiStateBase
{
    public BanListEuiState(string banListPlayerName, List<SharedBan> bans, List<SharedBan> roleBans)
    {
        BanListPlayerName = banListPlayerName;
        Bans = bans;
        RoleBans = roleBans;
    }

    /// <summary>
    /// The player name to use for the UI window title.
    /// </summary>
    public string BanListPlayerName { get; }

    /// <summary>
    /// List of bans for the player.
    /// </summary>
    public List<SharedBan> Bans { get; }

    /// <summary>
    /// List of rolebans for the player.
    /// </summary>
    public List<SharedBan> RoleBans { get; }
}
