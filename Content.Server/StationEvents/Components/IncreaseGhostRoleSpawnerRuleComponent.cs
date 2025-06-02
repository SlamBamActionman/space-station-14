using Content.Server.StationEvents.Events;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(IncreaseGhostRoleSpawnerRule))]
public sealed partial class IncreaseGhostRoleSpawnerRuleComponent : Component
{
    /// <summary>
    /// Spawner prototypes this game rule affects.
    /// </summary>
    [DataField]
    public List<EntProtoId> EligibleSpawners = new();

    /// <summary>
    /// The number of new ghost roles to be made available at spawners.
    /// If TargetAllSpawners is true, this value gets added to every spawner.
    /// Otherwise, this value of roles gets distributed between all spawners.
    /// </summary>
    [DataField]
    public int AddedRoleCount = 1;

    /// <summary>
    /// If true, AddedRoleCount is added to all eligible spawners.
    /// If false, AddedRoleCount is randomly distributed between all spawners.
    /// </summary>
    [DataField]
    public bool TargetAllSpawners = false;
}
