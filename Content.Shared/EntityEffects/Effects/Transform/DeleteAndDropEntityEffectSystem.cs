using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects.Transform;

/// <summary>
/// Deletes the affected entity and drops any items they may have been holding/wearing. Useful for e.g. ashing.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T, TEffect}"/>
public sealed partial class DeleteAndDropEntityEffectSystem : EntityEffectSystem<TransformComponent, DeleteAndDrop>
{
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private InventorySystem _inventory = default!;
    [Dependency] private SharedPopupSystem _popup = default!;

    protected override void Effect(Entity<TransformComponent> entity, ref EntityEffectEvent<DeleteAndDrop> args)
    {
        if (TryComp<InventoryComponent>(entity, out var comp))
        {
            foreach (var item in _inventory.GetHandOrInventoryEntities((entity.Owner, null, comp)))
            {
                _transform.DropNextTo(item, entity.AsNullable());
            }
        }

        var bodyIdentity = Identity.Entity(entity, EntityManager);
        if (args.Effect.PopupMessage != null)
            _popup.PopupCoordinates(Loc.GetString(args.Effect.PopupMessage, ("name", bodyIdentity)), _transform.GetMoverCoordinates(entity), PopupType.LargeCaution);

        PredictedDel(entity.Owner);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class DeleteAndDrop : EntityEffectBase<DeleteAndDrop>
{
    /// <summary>
    /// The popup displayed upon destruction. Can be null.
    /// </summary>
    [DataField]
    public LocId? PopupMessage = "bodyburn-text-others";

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) => null;
}
