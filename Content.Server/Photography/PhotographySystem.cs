using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Photography;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Coordinates;
using Robust.Server.GameObjects;

namespace Content.Server.Photography;


public sealed partial class PhotographySystem : EntitySystem
{

    [Dependency] private readonly SharedChargesSystem _charges = default!;
    [Dependency] private readonly PhotoSystem _photoSystem = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PhotographyComponent, MeleeHitEvent>(OnCameraMeleeHit);
        SubscribeLocalEvent<PhotographyComponent, UseInHandEvent>(OnCameraUseInHand);
    }

    private void OnCameraMeleeHit(Entity<PhotographyComponent> ent, ref MeleeHitEvent args)
    {
        TakePhoto(ent, args.User);
    }

    private void OnCameraUseInHand(Entity<PhotographyComponent> ent, ref UseInHandEvent args)
    {
        TakePhoto(ent, args.User);
    }

    private bool TakePhoto(Entity<PhotographyComponent> ent, EntityUid user)
    {
        /*TryComp<LimitedChargesComponent>(ent.Owner, out var charges);
        if (_charges.IsEmpty(ent.Owner, charges))
            return false;*/
        var photo = Spawn("Photo", _transform.GetMapCoordinates(ent.Owner));
        EnsureComp(photo, out PhotoSessionComponent comp);
        _photoSystem.InitializePhoto(user, comp);

        return true;
    }
}
