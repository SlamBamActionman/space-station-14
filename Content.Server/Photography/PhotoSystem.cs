using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Emp;
using Content.Server.Maps;
using Content.Server.Power.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.DeviceNetwork;
using Content.Shared.Interaction;
using Content.Shared.Photography;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Content.Server.Polymorph.Systems;
using Content.Shared.Polymorph;
using Content.Shared.Polymorph.Components;
using Content.Shared.Polymorph.Systems;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Content.Shared.Item;
using Robust.Shared.Utility;
using Robust.Shared.Enums;
using Robust.Shared.GameStates;

namespace Content.Server.Photography;

public sealed partial class PhotoSystem : SharedPhotoSystem
{

    [Dependency] private readonly ViewSubscriberSystem _viewSubscriberSystem = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly EyeSystem _eye = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    [Dependency] private readonly SharedSpriteSaverSystem _spriteSaverSystem = default!;

    public override void Initialize()
    {
        /*SubscribeLocalEvent<PhotoComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<PhotoComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<PhotoComponent, ComponentInit>(OnComponentInit);*/
        SubscribeNetworkEvent<PhotoStopViewingEvent>(OnStopViewing);
        SubscribeNetworkEvent<ProvidePhotoRotationEvent>(OnRotationProvided);
        SubscribeLocalEvent<PhotoSessionComponent, ActivateInWorldEvent>(OnPhotoActivate);
        SubscribeLocalEvent<PhotoViewerComponent, PlayerDetachedEvent>(OnViewerDetached);
        SubscribeLocalEvent<PhotoSessionComponent, ComponentShutdown>(OnSessionShutdown);
        SubscribeLocalEvent<PhotoViewerComponent, ComponentShutdown>(OnViewerShutdown);
        SubscribeLocalEvent<PhotoSessionComponent, GetVerbsEvent<ActivationVerb>>(AddPlayGameVerb);
        SubscribeLocalEvent<AppearanceCopyComponent, ComponentGetState>(OnGetState);

        InitializeMap();
    }

    public void OnGetState(EntityUid uid, AppearanceCopyComponent component, ref ComponentGetState args)
    {
        args.State = new AppearanceCopyComponentState(component.PrototypeId);
    }

    public void InitializePhoto(EntityUid initializer, PhotoSessionComponent comp)
    {
        if (!EntityManager.TryGetComponent(initializer, out ActorComponent? actor))
            return;

        EnsureSession(comp, actor.PlayerSession);
    }

    private void OnSessionShutdown(Entity<PhotoSessionComponent> ent, ref ComponentShutdown args)
    {
        CleanupSession(ent.Owner);
    }

    private void OnViewerShutdown(Entity<PhotoViewerComponent> ent, ref ComponentShutdown args)
    {
        if (!EntityManager.TryGetComponent(ent.Owner, out ActorComponent? actor))
            return;

        if (ent.Comp.Photo.IsValid())
            CloseSessionFor(actor.PlayerSession, ent.Comp.Photo);
    }

    private void OnPhotoActivate(EntityUid uid, PhotoSessionComponent component, ActivateInWorldEvent args)
    {
        // Check that a player is attached to the entity.
        if (!EntityManager.TryGetComponent(args.User, out ActorComponent? actor))
            return;

        OpenSessionFor(actor.PlayerSession, uid);
    }

    private void OnStopViewing(PhotoStopViewingEvent msg, EntitySessionEventArgs args)
    {
        CloseSessionFor(args.SenderSession, GetEntity(msg.PhotoUid));
    }

    private void OnViewerDetached(EntityUid uid, PhotoViewerComponent component, PlayerDetachedEvent args)
    {
        if (component.Photo.IsValid())
            CloseSessionFor(args.Player, component.Photo);
    }

    private void OnRotationProvided(ProvidePhotoRotationEvent msg)
    {
        if (TryComp(GetEntity(msg.PhotoUid), out PhotoSessionComponent? comp)) {
            comp.CameraAngle = msg.Rotation;
        }
    }

    private void AddPlayGameVerb(EntityUid uid, PhotoSessionComponent component, GetVerbsEvent<ActivationVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!EntityManager.TryGetComponent(args.User, out ActorComponent? actor))
            return;

        var playVerb = new ActivationVerb()
        {
            Text = "Debug View Photo",
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/die.svg.192dpi.png")),
            Act = () => OpenSessionFor(actor.PlayerSession, uid)
        };

        args.Verbs.Add(playVerb);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PhotoViewerComponent>();
        while (query.MoveNext(out var uid, out var viewer))
        {
            if (!Exists(viewer.Photo))
                continue;

            if (!TryComp(uid, out ActorComponent? actor))
            {
                EntityManager.RemoveComponent<PhotoViewerComponent>(uid);
                return;
            }

            if (actor.PlayerSession.Status != SessionStatus.InGame || !CanSeePhoto(uid, viewer.Photo))
                CloseSessionFor(actor.PlayerSession, viewer.Photo);
        }
    }
}
