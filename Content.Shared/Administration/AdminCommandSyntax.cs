namespace Content.Shared.Administration;

/// <summary>
/// Command names that may be used programmatically (e.g. in cmdlinks). In their own class to ensure consistency.
/// </summary>
public static class AdminCommandSyntax
{
    /// <summary>
    /// Command to open up the player panel for a user.
    /// </summary>
    public const string NamePlayerPanel = "playerpanel";

    /// <summary>
    /// Command to follow an entity.
    /// </summary>
    public const string NameFollow = "follow";

    /// <summary>
    /// Command to open the ahelp window for a user.
    /// </summary>
    public const string NameOpenAdminHelp = "openahelp";
}
