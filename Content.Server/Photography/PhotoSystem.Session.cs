using System.Numerics;
using Content.Shared.GameTicking;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Content.Shared.Photography;
using Robust.Shared.Containers;
using Robust.Server.GameObjects;
using Serilog.Configuration;
using Content.Shared.Coordinates;
using Content.Shared.Humanoid;
using Robust.Shared.Physics.Systems;
using Robust.Shared.GameStates;
using System.Linq;
using System.Xml.Schema;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Content.Shared.Explosion.Components;
using Content.Server.Explosion.EntitySystems;

namespace Content.Server.Photography;
public sealed partial class PhotoSystem
{

    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly MapSystem _mapSystem = default!;
    [Dependency] private readonly SharedPhysicsSystem _physicsSystem = default!;
    [Dependency] private readonly SharedPointLightSystem _pointLightSystem = default!;
    [Dependency] private readonly ExplosionSystem _explosionSystem = default!;
    [Dependency] private readonly OccluderSystem _occluderSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;

    public PhotoSession EnsureSession(PhotoSessionComponent comp, ICommonSession player)
    {
        // We already have a session, return it
        // TODO: if tables are connected, treat them as a single entity. This can be done by sharing the session.
        if (comp.Session != null)
            return comp.Session;

        // We make sure that the tabletop map exists before continuing.
        EnsurePhotoMap();

        // Create new session.
        var session = new PhotoSession(PhotoMap, GetNextTabletopPosition());
        comp.Session = session;

        Dictionary<EntityUid, MapGridComponent> gridDictionary = new();
        var centerpos = _transformSystem.GetWorldPosition(comp.Owner);

        // Since this is the first time opening this session, set up the game
        var entities = GetPhotoEntitiesInRange(comp.Owner, 8, 15, LookupFlags.Uncontained);

        RaiseNetworkEvent(new QueryPhotoRotationEvent(GetNetEntity(comp.Owner)), player.Channel);

        var mapId = comp.Owner.ToCoordinates().GetMapId(EntityManager);

        var photoArea = new Box2(comp.Owner.ToCoordinates().ToMapPos(EntityManager) - new Vector2(5, 5), comp.Owner.ToCoordinates().ToMapPos(EntityManager) + new Vector2(5, 5));

        var intersectingGrids = _mapManager.FindGridsIntersecting(mapId, photoArea);

        foreach (var grid in intersectingGrids)
        {
            var gridUid = grid.Owner;

            if (!TryComp<TransformComponent>(gridUid, out TransformComponent? gridTransform))
                continue;

            var gridPosRot = _transformSystem.GetWorldPositionRotation(gridTransform!);

            var fakeGrid = _mapManager.CreateGridEntity(PhotoMap);
            var fakexform = EnsureComp<TransformComponent>(fakeGrid.Owner);
            _physicsSystem.SetCanCollide(fakeGrid.Owner, false, true, true);
            if (HasComp<PhysicsComponent>(fakeGrid.Owner))
                _physicsSystem.SetBodyType(fakeGrid.Owner, BodyType.Static);


            gridDictionary.Add(gridUid, fakeGrid.Comp);

            AddTilesFromGrid(fakeGrid, gridUid, grid, photoArea, gridPosRot.WorldRotation, session.Position.Offset(gridPosRot.WorldPosition - centerpos).Position);
        }

        foreach (var ent in entities)
        {
            var pos = _transformSystem.GetWorldPosition(ent) - centerpos;

            var fakeItem = EntityManager.SpawnEntity("PhotoFakeItem", session.Position.Offset(pos));
            session.Entities.Add(fakeItem);

            Logger.Debug(fakeItem.ToString());

            //Rotation
            _transformSystem.SetWorldRotation(fakeItem, _transformSystem.GetWorldRotation(ent));

            //Appearance data
            _appearanceSystem.CopyData(ent, fakeItem);

            //Sprite details 
            /*var spriteSaverComp = EnsureComp<SpriteSaverComponent>(fakeItem);
            _spriteSaverSystem.SetSourceEntity(fakeItem, ent, player);*/
            var appearanceCopyComp = EnsureComp<AppearanceCopyComponent>(fakeItem);
            if (TryComp(ent, out MetaDataComponent? metaComp))
            {
                Logger.Debug(metaComp.EntityName);
                if (metaComp.EntityPrototype != null)
                {
                    Logger.Debug(metaComp.EntityPrototype.ID);
                    appearanceCopyComp.PrototypeId = metaComp.EntityPrototype.ID;
                    Dirty(appearanceCopyComp);
                    //_appearanceSystem.SetData(fakeItem, AppearanceCopyVisuals.Prototype, metaComp.EntityPrototype.ID);
                }
            }

            //_appearanceSystem.SetData(fakeItem, AppearanceCopyVisuals.Prototype, EntityManager.)


            // Explosion visualization
            if (TryComp(ent, out ExplosionVisualsComponent? explosionComp))
            {
                var fakeExplosionVisualsComp = EnsureComp<ExplosionVisualsComponent>(fakeItem);
                fakeExplosionVisualsComp.Epicenter = session.Position.Offset(pos);
                fakeExplosionVisualsComp.ExplosionType = explosionComp.ExplosionType;
                fakeExplosionVisualsComp.Intensity = explosionComp.Intensity;
                fakeExplosionVisualsComp.SpaceMatrix = explosionComp.SpaceMatrix;
                fakeExplosionVisualsComp.SpaceMatrix.Translation = session.Position.Position - centerpos;
                fakeExplosionVisualsComp.SpaceTiles = explosionComp.SpaceTiles;
                fakeExplosionVisualsComp.SpaceTileSize = explosionComp.SpaceTileSize;
                fakeExplosionVisualsComp.Animated = false;

                // Any tile-based explosion visuals are converted to space explosions and matched to the correct location.

                foreach (var (entity, data) in explosionComp.Tiles)
                {
                    var fakeItemTileExplosion = EntityManager.SpawnEntity("PhotoFakeItem", session.Position.Offset(pos));
                    session.Entities.Add(fakeItemTileExplosion);
                    _transformSystem.SetWorldRotation(fakeItemTileExplosion, _transformSystem.GetWorldRotation(ent));

                    _appearanceSystem.CopyData(ent, fakeItemTileExplosion);
                    var fakeTileExplosionVisualsComp = EnsureComp<ExplosionVisualsComponent>(fakeItemTileExplosion);
                    fakeTileExplosionVisualsComp.Epicenter = session.Position.Offset(pos);
                    fakeTileExplosionVisualsComp.ExplosionType = explosionComp.ExplosionType;
                    fakeTileExplosionVisualsComp.Intensity = explosionComp.Intensity; // Light is already created with the first ExplosionVisualsComponent.
                    fakeTileExplosionVisualsComp.SpaceTiles = data;
                    fakeTileExplosionVisualsComp.Animated = false;

                    if (!_entityManager.TryGetComponent(entity, out MapGridComponent? explosionGrid))
                        continue;
                    if (!_entityManager.TryGetComponent(entity, out TransformComponent? explosionXform))
                        continue;

                    fakeTileExplosionVisualsComp.SpaceTileSize = explosionGrid.TileSize;
                    fakeTileExplosionVisualsComp.SpaceMatrix = explosionXform.WorldMatrix;
                    fakeTileExplosionVisualsComp.SpaceMatrix.Translation += session.Position.Position - centerpos;
                }
            }

            // Point lights
            if (TryComp(ent, out PointLightComponent? lightComp))
            {
                var fakeLightComp = EnsureComp<PointLightComponent>(fakeItem);
                _pointLightSystem.SetCastShadows(fakeItem, lightComp.CastShadows);
                _pointLightSystem.SetColor(fakeItem, lightComp.Color);
                _pointLightSystem.SetEnabled(fakeItem, lightComp.Enabled);
                _pointLightSystem.SetEnergy(fakeItem, lightComp.Energy);
                _pointLightSystem.SetRadius(fakeItem, lightComp.Radius);
                _pointLightSystem.SetSoftness(fakeItem, lightComp.Softness);
                _pointLightSystem.SetMaskPath(fakeItem, lightComp.MaskPath);
                //fakeLightComp.NetSyncEnabled = lightComp.NetSyncEnabled;
                fakeLightComp.Offset = lightComp.Offset;
                fakeLightComp.Rotation = lightComp.Rotation;
                fakeLightComp.MaskAutoRotate = lightComp.MaskAutoRotate;
            }

            // Occlusion
            if (TryComp(ent, out OccluderComponent? occluderComp))
            {
                var fakeOccluderComp = EnsureComp<OccluderComponent>(fakeItem);
                _occluderSystem.SetBoundingBox(fakeItem, occluderComp.BoundingBox);
                _occluderSystem.SetEnabled(fakeItem, occluderComp.Enabled);
            }

            // Anchoring
            if (TryComp(ent, out TransformComponent? xform) && xform != null && xform.GridUid != null && xform.Anchored && gridDictionary.ContainsKey(xform.GridUid.Value))
            {
                EnsureComp(ent, out CollideOnAnchorComponent fakeCollideComp);
                fakeCollideComp.Enable = false;
                _transformSystem.AnchorEntity(fakeItem, EnsureComp<TransformComponent>(fakeItem), gridDictionary[xform.GridUid.Value]);
            }
        }

        Log.Info($"Created tabletop session number {comp} at position {session.Position}.");

        return session;
    }

    /// <summary>
    ///     Cleans up a tabletop game session, deleting every entity in it.
    /// </summary>
    /// <param name="uid">The UID of the tabletop game entity.</param>
    public void AddTilesFromGrid(Entity<MapGridComponent> targetGrid, EntityUid sourceGridUid, MapGridComponent sourceGridComponent, Box2 area, Angle rot, Vector2 pos)
    {
        foreach (var tile in _mapSystem.GetTilesIntersecting(sourceGridUid, sourceGridComponent, area, true))
        {
            _transformSystem.SetParent(targetGrid, _mapManager.GetMapEntityId(PhotoMap));

            _transformSystem.SetWorldRotationNoLerp(targetGrid.Owner, rot);
            _transformSystem.SetWorldPosition(targetGrid, pos);

            _mapSystem.SetTile(targetGrid, targetGrid, tile.GridIndices, tile.Tile);

        }
    }

    /// <summary>
    ///     Cleans up a tabletop game session, deleting every entity in it.
    /// </summary>
    /// <param name="uid">The UID of the tabletop game entity.</param>
    public void CleanupSession(EntityUid uid)
    {
        if (!EntityManager.TryGetComponent(uid, out PhotoSessionComponent? photo))
            return;

        if (photo.Session is not { } session)
            return;

        foreach (var (player, _) in session.Players)
        {
            CloseSessionFor(player, uid);
        }

        foreach (var euid in session.Entities)
        {
            EntityManager.QueueDeleteEntity(euid);
        }

        photo.Session = null;
    }

    /// <summary>
    ///     Adds a player to a tabletop game session, sending a message so the tabletop window opens on their end.
    /// </summary>
    /// <param name="player">The player session in question.</param>
    /// <param name="uid">The UID of the tabletop game entity.</param>
    public void OpenSessionFor(ICommonSession player, EntityUid uid)
    {
        if (!EntityManager.TryGetComponent(uid, out PhotoSessionComponent? photo) || player.AttachedEntity is not { Valid: true } attachedEntity)
            return;

        // Make sure we have a session, and add the player to it if not added already.
        var session = EnsureSession(photo, player);

        if (session.Players.ContainsKey(player))
            return;

        if (EntityManager.TryGetComponent(attachedEntity, out PhotoViewerComponent? viewer))
            CloseSessionFor(player, viewer.Photo, false);

        // Set the entity as an absolute GAMER.
        EnsureComp<PhotoViewerComponent>(attachedEntity).Photo = uid;

        // Create a camera for the gamer to use
        var camera = CreateCamera(photo, player);

        session.Players[player] = camera;

        // Tell the gamer to open a viewport for the tabletop game
        RaiseNetworkEvent(new PhotoViewEvent(GetNetEntity(uid), GetNetEntity(camera), photo.Size, photo.CameraAngle), player.Channel);
    }

    /// <summary>
    ///     Removes a player from a tabletop game session, and sends them a message so their tabletop window is closed.
    /// </summary>
    /// <param name="player">The player in question.</param>
    /// <param name="uid">The UID of the tabletop game entity.</param>
    /// <param name="removeGamerComponent">Whether to remove the <see cref="TabletopGamerComponent"/> from the player's attached entity.</param>
    public void CloseSessionFor(ICommonSession player, EntityUid uid, bool removeGamerComponent = true)
    {
        if (!EntityManager.TryGetComponent(uid, out PhotoSessionComponent? photo) || photo.Session is not { } session)
            return;

        if (!session.Players.TryGetValue(player, out var data))
            return;

        if (removeGamerComponent && player.AttachedEntity is { } attachedEntity && EntityManager.TryGetComponent(attachedEntity, out PhotoViewerComponent? viewer))
        {
            // We invalidate this to prevent an infinite feedback from removing the component.
            viewer.Photo = EntityUid.Invalid;

            // You stop being a gamer.......
            EntityManager.RemoveComponent<PhotoViewerComponent>(attachedEntity);
        }

        session.Players.Remove(player);
        session.Entities.Remove(data);

        // Deleting the view subscriber automatically cleans up subscriptions, no need to do anything else.
        EntityManager.QueueDeleteEntity(data);
    }

    /// <summary>
    ///     A helper method that creates a camera for a specified player, in a tabletop game session.
    /// </summary>
    /// <param name="tabletop">The tabletop game component in question.</param>
    /// <param name="player">The player in question.</param>
    /// <param name="offset">An offset from the tabletop position for the camera. Zero by default.</param>
    /// <returns>The UID of the camera entity.</returns>
    private EntityUid CreateCamera(PhotoSessionComponent photo, ICommonSession player, Vector2 offset = default)
    {
        DebugTools.AssertNotNull(photo.Session);

        var session = photo.Session!;

        // Spawn an empty entity at the coordinates
        var camera = EntityManager.SpawnEntity("PhotoCameraEntity", session.Position.Offset(offset));

        Logger.Debug(camera.ToString());

        // Add an eye component and disable FOV
        var eyeComponent = EnsureComp<EyeComponent>(camera);
        _eye.SetDrawFov(camera, true, eyeComponent);
        _eye.SetZoom(camera, photo.CameraZoom, eyeComponent);

        // Add the user to the view subscribers. If there is no player session, just skip this step
        _viewSubscriberSystem.AddViewSubscriber(camera, player);

        return camera;
    }

    public HashSet<EntityUid> GetPhotoEntitiesInRange(EntityUid uid, float visibleObjectRange, float lightObjectRange, LookupFlags flags = EntityLookupSystem.DefaultFlags)
    {
        var mapPos = _transformSystem.GetMapCoordinates(uid);

        if (mapPos.MapId == MapId.Nullspace)
            return [];

        var intersecting = _lookup.GetEntitiesInRange(mapPos, lightObjectRange, flags);
        foreach(var ent in intersecting)
        {
            var distance = _transformSystem.GetWorldPosition(uid) - _transformSystem.GetWorldPosition(ent);
            if (distance.Length() > visibleObjectRange)
            {
                if (!HasComp<OccluderComponent>(ent) && !HasComp<PointLightComponent>(ent))
                    intersecting.Remove(ent);
            }
        }
        intersecting.Remove(uid);
        return intersecting;
    }
}
