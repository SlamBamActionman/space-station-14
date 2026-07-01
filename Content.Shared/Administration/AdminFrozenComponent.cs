using Robust.Shared.GameStates;

namespace Content.Shared.Administration;

/// <summary>
/// Freezes a player stopping them from taking any actions. Optionally mutes them.
/// </summary>
[RegisterComponent, Access(typeof(AdminFrozenSystem))]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AdminFrozenComponent : Component
{
    /// <summary>
    /// Whether the player is also muted.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Muted;
}
