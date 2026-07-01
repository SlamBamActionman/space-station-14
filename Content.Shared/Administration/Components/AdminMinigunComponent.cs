using Robust.Shared.GameStates;

namespace Content.Shared.Administration.Components;

/// <summary>
/// Sets any gun with this component to have an extreme firerate.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AdminMinigunComponent : Component;
