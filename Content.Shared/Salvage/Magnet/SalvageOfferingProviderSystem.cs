using Content.Shared.Interaction;
using Robust.Shared.Containers;

namespace Content.Shared.Salvage.Magnet;

public sealed class SalvageOfferingProviderSystem : EntitySystem
{
    [Dependency] private readonly SharedSalvageSystem _salvage = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SalvageOfferingProviderComponent, EntInsertedIntoContainerMessage>(OnEntInserted);
    }

    private void OnEntInserted(Entity<SalvageOfferingProviderComponent> entity, ref EntInsertedIntoContainerMessage args)
    {
        if (!TryComp<SalvageMagnetComponent>(args.Container.Owner, out var magnet))
            return;
    }
}
