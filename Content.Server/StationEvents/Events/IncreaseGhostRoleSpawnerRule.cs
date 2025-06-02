using Content.Server.StationEvents.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Ghost.Roles.Components;
using Content.Shared.Station.Components;
using Content.Shared.Storage;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events;

public sealed class IncreaseGhostRoleSpawnerRule : StationEventSystem<IncreaseGhostRoleSpawnerRuleComponent>
{
    protected override void Started(EntityUid uid, IncreaseGhostRoleSpawnerRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        var spawners = EntityQueryEnumerator<GhostRoleMobSpawnerComponent, MetaDataComponent>();

        var eligibleSpawners = new List<Entity<GhostRoleMobSpawnerComponent>>();
        while (spawners.MoveNext(out var spawnerUid, out var spawnerComp, out var metadata))
        {
            if (metadata.EntityPrototype != null && component.EligibleSpawners.Contains(metadata.EntityPrototype.ID))
            {
                eligibleSpawners.Add((spawnerUid, spawnerComp));
            }
        }

        if (eligibleSpawners.Count == 0)
            return;

        if (component.TargetAllSpawners)
        {
            foreach (var spawner in eligibleSpawners)
            {
                spawner.Comp.AvailableTakeovers += component.AddedRoleCount;
            }
        }
        else
        {
            RobustRandom.Shuffle(eligibleSpawners);
            int i = 0;
            while (i < component.AddedRoleCount)
            {
                foreach (var spawner in eligibleSpawners)
                {
                    spawner.Comp.AvailableTakeovers++;
                    i++;
                    if (i >= component.AddedRoleCount)
                        break;
                }
            }
        }
    }
}
