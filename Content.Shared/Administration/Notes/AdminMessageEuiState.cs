using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

/// <summary>
/// EUI state for popping up admin messages to a player, e.g. giving a note when a player is logged out so they see it upon logging in.
/// </summary>
[Serializable, NetSerializable]
public sealed class AdminMessageEuiState(TimeSpan time, AdminMessageEuiState.Message[] messages) : EuiStateBase
{
    /// <summary>
    /// The time the user must wait before they close the message window.
    /// </summary>
    public TimeSpan Time { get; } = time;

    /// <summary>
    /// The messages included in the pop-up.
    /// </summary>
    public Message[] Messages { get; } = messages;

    /// <summary>
    /// Container for the admin message.
    /// </summary>
    /// <param name="text">Message text.</param>
    /// <param name="adminName">Name of the admin giving the message.</param>
    /// <param name="addedOn">When the message was added.</param>
    [Serializable]
    public sealed class Message(string text, string adminName, DateTime addedOn)
    {
        public string Text = text;
        public string AdminName = adminName;
        public DateTime AddedOn = addedOn;
    }
}

/// <summary>
/// EUI messages related to admin message pop-ups.
/// </summary>
public static class AdminMessageEuiMsg
{
    /// <summary>
    /// Message to dismiss the admin message pop-up.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class Dismiss(bool permanent) : EuiMessageBase
    {
        /// <summary>
        /// Whether the message should be dismissed permanently.
        /// </summary>
        public bool Permanent { get; } = permanent;
    }
}
