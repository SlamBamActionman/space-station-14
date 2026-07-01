using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Actions;

/// <summary>
/// Grants actions on MapInit and removes them on shutdown.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(ActionGrantSystem))]
public sealed partial class ActionGrantComponent : Component
{
    /// <summary>
    /// Actions to grant; created on map initiation.
    /// </summary>
    [DataField(required: true), AutoNetworkedField, AlwaysPushInheritance]
    public List<EntProtoId> Actions = new();

    /// <summary>
    /// Action entities that have been granted.
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<EntityUid> ActionEntities = new();
}
