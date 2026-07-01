using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared.Actions.Components;

/// <summary>
/// This component indicates that this entity contains actions inside of some container.
/// </summary>
[NetworkedComponent, RegisterComponent, Access(typeof(ActionContainerSystem), typeof(SharedActionsSystem))]
public sealed partial class ActionsContainerComponent : Component
{
    /// <summary>
    /// Action container ID string.
    /// </summary>
    public const string ContainerId = "actions";

    /// <summary>
    /// Container that contains the action entities.
    /// </summary>
    [ViewVariables]
    public Container Container = default!;
}
