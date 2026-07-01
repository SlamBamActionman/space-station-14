using System.Collections.Immutable;
using Content.Shared.Database;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

/// <summary>
/// Record containing data related to an admin note.
/// </summary>
/// <param name="Id">Id of note, message, watchlist, ban or role ban. Should be paired with NoteType to uniquely identify a shared admin note.</param>
/// <param name="Players">Note's player.</param>
/// <param name="Rounds">Which round was it added in?</param>
/// <param name="ServerName">Which server was this added on?</param>
/// <param name="PlaytimeAtNote">Playtime at the time of getting the note.</param>
/// <param name="NoteType">Type of note.</param>
/// <param name="Message">Attached message.</param>
/// <param name="NoteSeverity">Severity of the note, ban or role ban. Otherwise null.</param>
/// <param name="Secret">Is it visible to the player (only relevant if players can see their own notes)</param>
/// <param name="CreatedByName">Who created it?</param>
/// <param name="EditedByName">Who edited it last?</param>
/// <param name="CreatedAt">When was it created?</param>
/// <param name="LastEditedAt">When was it last edited?</param>
/// <param name="ExpiryTime">Does it expire?</param>
/// <param name="BannedRoles">Only valid for role bans. List of banned roles</param>
/// <param name="UnbannedTime">Only valid for bans. Set if unbanned.</param>
/// <param name="UnbannedByName">Only valid for bans. Set if unbanned.</param>
/// <param name="Seen">Only valid for messages, otherwise should be null. Has the user seen this message?</param>
[Serializable, NetSerializable]
public sealed record SharedAdminNote(
    int Id,
    ImmutableArray<NetUserId> Players,
    ImmutableArray<int> Rounds,
    string? ServerName,
    TimeSpan PlaytimeAtNote,
    NoteType NoteType,
    string Message,
    NoteSeverity? NoteSeverity,
    bool Secret,
    string CreatedByName,
    string EditedByName,
    DateTime CreatedAt,
    DateTime? LastEditedAt,
    DateTime? ExpiryTime,
    ImmutableArray<BanRoleDef>? BannedRoles,
    DateTime? UnbannedTime,
    string? UnbannedByName,
    bool? Seen
    );
