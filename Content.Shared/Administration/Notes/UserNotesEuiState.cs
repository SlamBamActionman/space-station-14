using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

/// <summary>
/// EUI state for a user's admin notes, as viewed by the user.
/// </summary>
[Serializable, NetSerializable]
public sealed class UserNotesEuiState : EuiStateBase
{
    /// <summary>
    /// EUI state for a user's admin notes.
    /// </summary>
    public UserNotesEuiState(Dictionary<(int, NoteType), SharedAdminNote> notes)
    {
        Notes = notes;
    }

    /// <summary>
    /// A user's notes, identified based on ID and note type.
    /// </summary>
    public Dictionary<(int, NoteType), SharedAdminNote> Notes { get; }
}
