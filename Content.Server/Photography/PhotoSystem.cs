using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Emp;
using Content.Server.Power.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.DeviceNetwork;
using Content.Shared.Interaction;
using Content.Shared.Photography;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Photography;

public sealed class PhotoSystem : EntitySystem
{

    [Dependency] private readonly ViewSubscriberSystem _viewSubscriberSystem = default!;
    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PhotoComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<PhotoComponent, GetVerbsEvent<UtilityVerb>>(OnUtilityVerb);
        SubscribeLocalEvent<PhotoComponent, InteractUsingEvent>(OnInteractUsing);

        Subs.BuiEvents<PhotoComponent>(PhotoUiKey.Photo, subs =>
        {
            subs.Event<BoundUIClosedEvent>(OnBoundUiClose);
        });
    }
    private void OnBoundUiClose(Entity<PhotoComponent> ent, ref BoundUIClosedEvent args)
    {
        RemoveActiveViewer(ent, args.Actor);
    }

    public void AddActiveViewer(Entity<PhotoComponent> ent, EntityUid player, ActorComponent? actor = null)
    {
        if (!Resolve(player, ref actor))
        {
            return;
        }

        _viewSubscriberSystem.AddViewSubscriber(ent.Owner, actor.PlayerSession); //Replace ent.Owner with ent.Comp.PhotoEntity
        ent.Comp.ActiveViewers.Add(player);
    }

    public void RemoveActiveViewer(Entity<PhotoComponent> ent, EntityUid player, ActorComponent? actor = null)
    {
        if (!Resolve(player, ref actor))
        {
            return;
        }

        _viewSubscriberSystem.RemoveViewSubscriber(ent.Owner, actor.PlayerSession); //Replace ent.Owner with ent.Comp.PhotoEntity
        ent.Comp.ActiveViewers.Remove(player);
    }

    private void OnUtilityVerb(Entity<PhotoComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var user = args.User;

        var verb = new UtilityVerb()
        {
            Act = () => ViewPhoto(ent, user),
            IconEntity = GetNetEntity(ent.Owner),
            Text = Loc.GetString("forensic-scanner-verb-text"),
            Message = Loc.GetString("forensic-scanner-verb-message")
        };

        args.Verbs.Add(verb);
    }

    private void OnInteractUsing(Entity<PhotoComponent> ent, ref InteractUsingEvent args)
    {
        ViewPhoto(ent, args.User);
    }

    private void ViewPhoto(Entity<PhotoComponent> ent, EntityUid player)
    {
        if (!_userInterface.TryOpenUi(ent.Owner, PhotoUiKey.Photo, player))
            return;
    }

    private void OnShutdown(Entity<PhotoComponent> ent, ref ComponentShutdown args)
    {
        Deactivate(ent);
    }

    private void Deactivate(Entity<PhotoComponent> ent)
    {

        var ev = new PhotoDeactivateEvent(ent);

        RemoveActiveViewers(ent, new(ent.Comp.ActiveViewers));

        // Send a local event that's broadcasted everywhere afterwards.
        RaiseLocalEvent(ev);
    }
    public void RemoveActiveViewers(Entity<PhotoComponent> ent, HashSet<EntityUid> players)
    {
        foreach (var player in players)
        {
            RemoveActiveViewer(ent, player);
        }
    }

    public sealed class PhotoDeactivateEvent : EntityEventArgs
    {
        public EntityUid Photo { get; }

        public PhotoDeactivateEvent(EntityUid photo)
        {
            Photo = photo;
        }
    }

}
