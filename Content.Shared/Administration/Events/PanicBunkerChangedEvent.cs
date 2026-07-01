using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events;

/// <summary>
/// Contains the data for the server's panic bunker.
/// </summary>
[Serializable, NetSerializable]
public sealed class PanicBunkerStatus
{
    /// <summary>
    /// Whether the bunker is enabled.
    /// </summary>
    public bool Enabled;

    /// <summary>
    /// If the bunker should be disabled when admins are online.
    /// </summary>
    public bool DisableWithAdmins;

    /// <summary>
    /// If the bunker should be enabled when admins are not online.
    /// </summary>
    public bool EnableWithoutAdmins;

    /// <summary>
    /// Whether de-adminned admins count as admins for the bunker evaluation.
    /// </summary>
    public bool CountDeadminnedAdmins;

    /// <summary>
    /// Whether the reason a player got denied by the bunker (e.g. playtime, account age) shoudl display, or just a non-specific message.
    /// </summary>
    public bool ShowReason;

    /// <summary>
    /// The required minimum account age to bypass the bunker.
    /// </summary>
    public int MinAccountAgeMinutes;

    /// <summary>
    /// The required minimum overall playtime to bypass the bunker.
    /// </summary>
    public int MinOverallMinutes;
}

[Serializable, NetSerializable]
public sealed class PanicBunkerChangedEvent : EntityEventArgs
{
    public PanicBunkerStatus Status;

    public PanicBunkerChangedEvent(PanicBunkerStatus status)
    {
        Status = status;
    }
}
