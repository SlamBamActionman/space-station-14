using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

/// <summary>
/// EUI state for a user's admin notes, as viewed by an admin.
/// </summary>
[Serializable, NetSerializable]
public sealed class AdminNotesEuiState : EuiStateBase
{
    /// <summary>
    /// EUI state for a user's admin notes, as viewed by an admin.
    /// </summary>
    public AdminNotesEuiState(string notedPlayerName, Dictionary<(int, NoteType), SharedAdminNote> notes, bool canCreate, bool canDelete, bool canEdit)
    {
        NotedPlayerName = notedPlayerName;
        Notes = notes;
        CanCreate = canCreate;
        CanDelete = canDelete;
        CanEdit = canEdit;
    }

    /// <summary>
    /// Name of the player, to be used in the window title.
    /// </summary>
    public string NotedPlayerName { get; }

    /// <summary>
    /// Dictionary of admin notes, identified by ID and note type.
    /// </summary>
    public Dictionary<(int noteId, NoteType noteType), SharedAdminNote> Notes { get; }

    /// <summary>
    /// Whether the user viewing the window can create notes.
    /// </summary>
    public bool CanCreate { get; }

    /// <summary>
    /// Whether the user viewing the window can delete notes.
    /// </summary>
    public bool CanDelete { get; }

    /// <summary>
    /// Whether the user viewing the window can edit notes.
    /// </summary>
    public bool CanEdit { get; }
}

/// <summary>
/// Class containing the various admin note messages an admin client would send to the server.
/// </summary>
public static class AdminNoteEuiMsg
{

    /// <summary>
    /// EUI message to create a new note.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class CreateNoteRequest : EuiMessageBase
    {
        /// <summary>
        /// EUI message to create a new note.
        /// </summary>
        public CreateNoteRequest(NoteType type, string message, NoteSeverity? severity, bool secret, DateTime? expiryTime)
        {
            NoteType = type;
            Message = message;
            NoteSeverity = severity;
            Secret = secret;
            ExpiryTime = expiryTime;
        }

        /// <summary>
        /// What type the note should be (e.g. note, ban, message etc.)
        /// </summary>
        public NoteType NoteType { get; set; }

        /// <summary>
        /// The message of the note.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The severity of the note.
        /// </summary>
        public NoteSeverity? NoteSeverity { get; set; }

        /// <summary>
        /// Whether the user will be able to read the note.
        /// </summary>
        public bool Secret { get; set; }

        /// <summary>
        /// When the note expires and is considered old.
        /// </summary>
        public DateTime? ExpiryTime { get; set; }
    }

    /// <summary>
    /// EUI message to delete a note. Requires both note ID and type to identify it.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class DeleteNoteRequest : EuiMessageBase
    {
        /// <summary>
        /// EUI message to delete a note. Requires both note ID and type to identify it.
        /// </summary>
        public DeleteNoteRequest(int id, NoteType type)
        {
            Id = id;
            Type = type;
        }

        /// <summary>
        /// The ID of the note.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The type of the note.
        /// </summary>
        public NoteType Type { get; set; }
    }

    /// <summary>
    /// EUI message to edit a note. Requires both note ID and type to identify it.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class EditNoteRequest : EuiMessageBase
    {
        /// <summary>
        /// EUI message to edit a note. Requires both note ID and type to identify it.
        /// </summary>
        public EditNoteRequest(int id, NoteType type, string message, NoteSeverity? severity, bool secret, DateTime? expiryTime)
        {
            Id = id;
            Type = type;
            Message = message;
            NoteSeverity = severity;
            Secret = secret;
            ExpiryTime = expiryTime;
        }

        /// <summary>
        /// The ID of the note.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The type of the note.
        /// </summary>
        public NoteType Type { get; set; }

        /// <summary>
        /// The new message of the note.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The severity of the note, if it should be changed.
        /// </summary>
        public NoteSeverity? NoteSeverity { get; set; }

        /// <summary>
        /// Whether the user will be able to read the note.
        /// </summary>
        public bool Secret { get; set; }

        /// <summary>
        /// When the note expires and is considered old.
        /// </summary>
        public DateTime? ExpiryTime { get; set; }
    }
}
