using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Humanoid.Components;

/// <summary>
///     This is added to a marker entity in order to spawn a randomized
///     humanoid ingame.
/// </summary>
[RegisterComponent, EntityCategory("Spawner")]
public sealed partial class RandomHumanoidSpawnerComponent : Component
{
    [DataField("settings", customTypeSerializer: typeof(PrototypeIdSerializer<RandomHumanoidSettingsPrototype>))]
    public string? SettingsPrototypeId;

    /// <summary>
    /// If true, the spawner doesn't spawn the humanoid until it itself is selected as a ghost role.
    /// It then transfers over the ghost role to the humanoid.
    /// </summary>
    [DataField]
    public bool WaitForGhostRole = false;
}
