using Robust.Shared.Serialization;

namespace Content.Shared.Administration.BanList;

/// <summary>
/// Record containing information for a single unban. Held by the related ban.
/// </summary>
/// <param name="UnbanningAdmin">Name of the unbanning admin.</param>
/// <param name="UnbanTime">When the unban was made.</param>
[Serializable, NetSerializable]
public sealed record SharedUnban(
    string? UnbanningAdmin,
    DateTime UnbanTime
);
