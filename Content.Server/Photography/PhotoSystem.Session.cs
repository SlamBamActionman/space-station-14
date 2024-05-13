using System.Numerics;
using Content.Shared.GameTicking;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Content.Shared.Photography;
using Robust.Shared.GameObjects;

namespace Content.Server.Photography;
public sealed partial class PhotoSystem
{
    public PhotoSession EnsureSession(PhotoSessionComponent comp)
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

        // Since this is the first time opening this session, set up the game
        //SetupTabletop(session, EntityManager); THIS MUST BE ADDED TO MAKE THE THINGS HAPPEN!!!!
        var board = EntityManager.SpawnEntity("PhotoFakeItem", session.Position.Offset(0, 0));
        session.Entities.Add(board);
        var spriteSaverComp = EnsureComp<SpriteSaverComponent>(board);
        _spriteSaverSystem.SetSourceEntity(board, comp.Owner);
        _appearanceSystem.CopyData(comp.Owner, board);

        Log.Info($"Created tabletop session number {comp} at position {session.Position}.");

        return session;
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
        var session = EnsureSession(photo);

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
        RaiseNetworkEvent(new PhotoViewEvent(GetNetEntity(uid), GetNetEntity(camera), photo.Size), player.Channel);
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
        var camera = EntityManager.SpawnEntity(null, session.Position.Offset(offset));

        // Add an eye component and disable FOV
        var eyeComponent = EnsureComp<EyeComponent>(camera);
        _eye.SetDrawFov(camera, false, eyeComponent);
        _eye.SetZoom(camera, photo.CameraZoom, eyeComponent);

        // Add the user to the view subscribers. If there is no player session, just skip this step
        _viewSubscriberSystem.AddViewSubscriber(camera, player);

        return camera;
    }
}
