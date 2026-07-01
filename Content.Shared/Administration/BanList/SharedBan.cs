using System.Collections.Immutable;
using Content.Shared.Database;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.BanList;

/// <summary>
/// Record containing information for a single ban instance.
/// </summary>
/// <param name="Id">ID for the ban.</param>
/// <param name="Type">Server/role ban.</param>
/// <param name="UserIds">User IDs associated with the ban.</param>
/// <param name="Addresses">Addresses associated with the ban.</param>
/// <param name="HWIds">HWIDs associated with the ban.</param>
/// <param name="BanTime">When the ban was placed.</param>
/// <param name="ExpirationTime">When the ban is considered expired, if ever.</param>
/// <param name="Reason">Reason provided for the ban.</param>
/// <param name="BanningAdminName">Name of the banning admin.</param>
/// <param name="Unban">Related unban, if any.</param>
/// <param name="Roles">Roles related to the ban if roleban.</param>
[Serializable, NetSerializable]
public record SharedBan(
    int? Id,
    BanType Type,
    ImmutableArray<NetUserId> UserIds,
    ImmutableArray<(string address, int cidrMask)> Addresses,
    ImmutableArray<string> HWIds,
    DateTime BanTime,
    DateTime? ExpirationTime,
    string Reason,
    string? BanningAdminName,
    SharedUnban? Unban,
    ImmutableArray<BanRoleDef>? Roles);
