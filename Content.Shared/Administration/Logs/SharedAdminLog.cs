using Content.Shared.Database;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs;

/// <summary>
/// Record containing the data of an admin log.
/// </summary>
/// <param name="Id">ID of the log.</param>
/// <param name="Type">Type/category of the log.</param>
/// <param name="Impact">Impact/severity of the log.</param>
/// <param name="Date">When the log was made.</param>
/// <param name="Message">The message of the admin log.</param>
/// <param name="Players"><see cref="Guid"/> of the players that the log relates to, for searching purposes.</param>
[Serializable, NetSerializable]
public readonly record struct SharedAdminLog(
    int Id,
    LogType Type,
    LogImpact Impact,
    DateTime Date,
    string Message,
    Guid[] Players);
