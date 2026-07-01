using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Content.Shared.Database;

namespace Content.Shared.Administration.Logs;

public interface ISharedAdminLogManager
{
    /// <summary>
    /// Determines if logs should be made. Controlled via <see cref="CCVars.AdminLogsEnabled"/>.
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// Converts the name following a specific JsonNamingPolicy, if applicable.
    /// </summary>
    /// <remarks>JsonNamingPolicy is not whitelisted by the sandbox.</remarks>
    public string ConvertName(string name);

    // Required for the log string interpolation handler to access ToPrettyString()
    /// <summary>
    /// Stores an IEntityManager dependency for easy access.
    /// </summary>
    public IEntityManager EntityManager { get; }

    /// <summary>
    /// Add an admin log, if logging is enabled.
    /// </summary>
    /// <param name="type">The type (or category) of log.</param>
    /// <param name="impact">The impact/severity of the log.</param>
    /// <param name="handler">The string handler; can contain markers to distinguish entities.</param>
    /// <remarks>The handler allows extra information to be included with the string. E.g: <c>$"{ToPrettyString(pullerUid):user} stopped pulling {ToPrettyString(pullableUid):target}."</c></remarks>
    void Add(LogType type, LogImpact impact, [InterpolatedStringHandlerArgument("")] ref LogStringHandler handler);

    /// <summary>
    /// Add an admin log, if logging is enabled. Defaults to the severity of <see cref="LogImpact.Medium"/>.
    /// </summary>
    /// <param name="type">The type (or category) of log.</param>
    /// <param name="handler">The string handler; can contain markers to distinguish entities.</param>
    /// <remarks>The handler allows extra information to be included with the string. E.g: <c>$"{ToPrettyString(pullerUid):user} stopped pulling {ToPrettyString(pullableUid):target}."</c></remarks>
    void Add(LogType type, [InterpolatedStringHandlerArgument("")] ref LogStringHandler handler);
}
