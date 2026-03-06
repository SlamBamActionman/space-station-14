using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared.Materials;

/// <summary>
/// If reclaimed, this entity will triggers its explosive component.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ExplodeOnReclaimComponent : Component
{
    /// <summary>
    /// If the whitelist is set, the reclaimer must pass it to trigger the explosion.
    /// </summary>
    [DataField]
    public EntityWhitelist? ReclaimerWhitelist;
}
