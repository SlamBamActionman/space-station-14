using Content.Shared.Access.Systems;
using Content.Shared.UserInterface;
using Robust.Shared.GameStates;

namespace Content.Shared.Access.Components;

/// <summary>
/// Stops the entity's <see cref="ActivatableUIComponent"/> from opening unless the user has the correct access.
/// Requires <see cref="AccessComponent"/> and <see cref="ActivatableUIComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(ActivatableUIRequiresAccessSystem))]
public sealed partial class ActivatableUIRequiresAccessComponent : Component
{
    /// <summary>
    /// Message to show if the access is missing.
    /// </summary>
    [DataField]
    public LocId? PopupMessage = "lock-comp-has-user-access-fail";
}
