using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs;

/// <summary>
/// EUI state for the admin log window.
/// </summary>
[Serializable, NetSerializable]
public sealed class AdminLogsEuiState : EuiStateBase
{
    /// <summary>
    /// EUI state for the admin log window.
    /// </summary>
    public AdminLogsEuiState(int roundId, Dictionary<Guid, string> players, int roundLogs)
    {
        RoundId = roundId;
        Players = players;
        RoundLogs = roundLogs;
    }

    /// <summary>
    /// Indicates logs are still loading from the database.
    /// </summary>
    public bool IsLoading { get; set; }

    /// <summary>
    /// Round ID that logs are being retrieved from.
    /// </summary>
    public int RoundId { get; }

    /// <summary>
    /// The list of players in the round.
    /// </summary>
    public Dictionary<Guid, string> Players { get; }

    /// <summary>
    /// The number of logs in the round.
    /// </summary>
    public int RoundLogs { get; }
}

/// <summary>
/// EUI messages related to admin logs.
/// </summary>
public static class AdminLogsEuiMsg
{
    /// <summary>
    /// Message to set the log filter of admin logs, from the server to the client.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class SetLogFilter : EuiMessageBase
    {
        /// <summary>
        /// Message to set the log filter of admin logs.
        /// </summary>
        public SetLogFilter(string? search = null, bool invertTypes = false, HashSet<LogType>? types = null)
        {
            Search = search;
            InvertTypes = invertTypes;
            Types = types;
        }

        /// <summary>
        /// The string to search for.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// If the selected log types should be inverted for the search.
        /// </summary>
        public bool InvertTypes { get; set; }

        /// <summary>
        /// The selected log types for the search.
        /// </summary>
        public HashSet<LogType>? Types { get; set; }
    }

    /// <summary>
    /// Message to display new admin logs in the logging UI.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class NewLogs : EuiMessageBase
    {
        /// <summary>
        /// Message to display new admin logs in the logging UI.
        /// </summary>
        public NewLogs(List<SharedAdminLog> logs, bool replace, bool hasNext)
        {
            Logs = logs;
            Replace = replace;
            HasNext = hasNext;
        }

        /// <summary>
        /// List of logs to display.
        /// </summary>
        public List<SharedAdminLog> Logs { get; set; }

        /// <summary>
        /// Whether the logs should replace the existing ones, or just add onto the window.
        /// </summary>
        public bool Replace { get; set; }

        /// <summary>
        /// Whether there are more admin logs to display, i.e. enable the Next button.
        /// </summary>
        public bool HasNext { get; set; }
    }

    /// <summary>
    /// Message request from the client to server on what logs to retrieve.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class LogsRequest : EuiMessageBase
    {
        /// <summary>
        /// Message request from the client to server on what logs to retrieve.
        /// </summary>
        public LogsRequest(
            int? roundId,
            string? search,
            HashSet<LogType>? types,
            HashSet<LogImpact>? impacts,
            DateTime? before,
            DateTime? after,
            bool includePlayers,
            Guid[]? anyPlayers,
            Guid[]? allPlayers,
            bool includeNonPlayers,
            DateOrder dateOrder)
        {
            RoundId = roundId;
            Search = search;
            Types = types;
            Impacts = impacts;
            Before = before;
            After = after;
            IncludePlayers = includePlayers;
            AnyPlayers = anyPlayers is { Length: > 0 } ? anyPlayers : null;
            AllPlayers = allPlayers is { Length: > 0 } ? allPlayers : null;
            IncludeNonPlayers = includeNonPlayers;
            DateOrder = dateOrder;
        }

        /// <summary>
        /// The round ID for the logs.
        /// </summary>
        public int? RoundId { get; set; }

        /// <summary>
        /// Any text string to search for in the logs.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// The log types to request.
        /// </summary>
        public HashSet<LogType>? Types { get; set; }

        /// <summary>
        /// The severity of logs to request.
        /// </summary>
        public HashSet<LogImpact>? Impacts { get; set; }

        /// <summary>
        /// Filter for logs before a certain time.
        /// </summary>
        public DateTime? Before { get; set; }

        /// <summary>
        /// Filter for logs after a certain time.
        /// </summary>
        public DateTime? After { get; set; }

        /// <summary>
        /// Whether for the filter to care about <see cref="AnyPlayers"/> and  <see cref="AllPlayers"/>.
        /// </summary>
        public bool IncludePlayers { get; set; }

        /// <summary>
        /// OR filter of players connected to the log.
        /// </summary>
        public Guid[]? AnyPlayers { get; set; }

        /// <summary>
        /// AND filter of players connected to the log.
        /// </summary>
        public Guid[]? AllPlayers { get; set; }

        /// <summary>
        /// Whether to include non-player logs.
        /// </summary>
        public bool IncludeNonPlayers { get; set; }

        /// <summary>
        /// Whether to sort in ascending or descending date.
        /// </summary>
        public DateOrder DateOrder { get; set; }
    }

    /// <summary>
    /// Message request by the client to display the next set of logs.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class NextLogsRequest : EuiMessageBase
    {
    }
}
