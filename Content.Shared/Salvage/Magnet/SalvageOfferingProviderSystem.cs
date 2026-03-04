using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.Magnet;

public sealed class SalvageOfferingProviderSystem : EntitySystem
{
    [Dependency] private readonly SharedSalvageSystem _salvage = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SalvageOfferingProviderComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<SalvageOfferingProviderComponent, LockToggledEvent>(OnUnlocked);
        SubscribeLocalEvent<SalvageOfferingProviderComponent, EntGotInsertedIntoContainerMessage>(OnEntInserted);
        SubscribeLocalEvent<SalvageOfferingProviderComponent, EntGotRemovedFromContainerMessage>(OnEntRemoved);
        SubscribeLocalEvent<SalvageOfferingProviderComponent, ContainerGettingInsertedAttemptEvent>(OnEntInsertedAttempt);
    }

    private void OnMapInit(Entity<SalvageOfferingProviderComponent> entity, ref MapInitEvent args)
    {
        entity.Comp.Offering = _random.Next(4, 7);
        var map = _salvage.TestGetSalvageMapPrototype(entity.Comp.Offering);

        if (map.JobConnection != null)
        {
            var job = _proto.Index(map.JobConnection);
            _appearance.SetData(entity, SalvageOfferingProviderVisuals.JobIcon, job.Icon.ToString());
        }
    }

    private void OnUnlocked(Entity<SalvageOfferingProviderComponent> entity, ref LockToggledEvent args)
    {
        entity.Comp.Available = !args.Locked;
    }

    private void OnEntInsertedAttempt(Entity<SalvageOfferingProviderComponent> entity, ref ContainerGettingInsertedAttemptEvent args)
    {
        if (TryComp<SalvageMagnetComponent>(args.Container.Owner, out var magnet) && !entity.Comp.Available)
        {
            // SLAM-NOTE: Gotta love not passing in the user for prediction. Slart is right about everything. :(
            _popup.PopupPredicted("Shuttle manifest not unlocked.", entity.Owner, null);
            args.Cancel();
        }
    }

    private void OnEntInserted(Entity<SalvageOfferingProviderComponent> entity, ref EntGotInsertedIntoContainerMessage args)
    {
        if (!TryComp<SalvageMagnetComponent>(args.Container.Owner, out var magnet))
            return;

        var ev = new ExtraEntryChangedEvent(entity.Comp.Offering);
        RaiseLocalEvent(args.Container.Owner, ref ev);
    }

    private void OnEntRemoved(Entity<SalvageOfferingProviderComponent> entity, ref EntGotRemovedFromContainerMessage args)
    {
        if (!TryComp<SalvageMagnetComponent>(args.Container.Owner, out var magnet))
            return;

        var ev = new ExtraEntryChangedEvent(0);
        RaiseLocalEvent(args.Container.Owner, ref ev);
    }
}

[Serializable, NetSerializable]
public enum SalvageOfferingProviderVisuals : byte
{
    IsLocked,
    JobIcon,
}

/// <summary>
/// SLAM-NOTE: Temporary playtesting event. Replace with proper value handling.
/// </summary>
[ByRefEvent]
public readonly struct ExtraEntryChangedEvent
{
    public readonly int ExtraEntry;

    public ExtraEntryChangedEvent(int extraEntry)
    {
        ExtraEntry = extraEntry;
    }
}
