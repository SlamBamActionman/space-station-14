using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
    /// <summary>
    /// Whether the in-world announcement should be station-only or server-wide.
    /// </summary>
    public enum AdminAnnounceType
    {
        Station,
        Server,
    }

    [Serializable, NetSerializable]
    public sealed class AdminAnnounceEuiState : EuiStateBase
    {
    }

    /// <summary>
    /// Message from the client to the server to perform an in-world announcement.
    /// </summary>
    public static class AdminAnnounceEuiMsg
    {
        [Serializable, NetSerializable]
        public sealed class DoAnnounce : EuiMessageBase
        {
            /// <summary>
            /// Whether the announcement window should close after clicking send.
            /// </summary>
            public bool CloseAfter;

            /// <summary>
            /// The announcer of the message.
            /// </summary>
            public string Announcer = default!;

            /// <summary>
            /// The announcement being made.
            /// </summary>
            public string Announcement = default!;

            /// <summary>
            /// Which type of announcement is being made.
            /// </summary>
            public AdminAnnounceType AnnounceType;
        }
    }
}
