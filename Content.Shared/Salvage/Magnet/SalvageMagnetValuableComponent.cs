using Robust.Shared.GameStates;

namespace Content.Shared.Salvage.Magnet;

/// <summary>
/// Indicates the entity is to be considered "valuable" to the magnet, and will assign itself to whatever wreck spawns it.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SalvageMagnetValuableComponent : Component
{
    /// <summary>
    /// Entity that spawned us.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? DataTarget;
}
