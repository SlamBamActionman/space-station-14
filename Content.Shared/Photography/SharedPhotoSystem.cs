using Robust.Shared.GameStates;
using Content.Shared.ActionBlocker;
using Robust.Shared.Serialization;
using Content.Shared.Interaction;
using Robust.Shared.Map;

namespace Content.Shared.Photography;

public abstract class SharedPhotoSystem : EntitySystem
{
    [Dependency] private readonly SharedInteractionSystem _interactionSystem = default!;
    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;

    public bool CanSeePhoto(EntityUid playerEntity, EntityUid? photo)
    {
        // Photo may have been deleted, hence TryComp
        if (!TryComp(photo, out MetaDataComponent? meta)
            || meta.EntityLifeStage >= EntityLifeStage.Terminating
            || (meta.Flags & MetaDataFlags.InContainer) == MetaDataFlags.InContainer)
        {
            return false;
        }

        return _interactionSystem.InRangeUnobstructed(playerEntity, photo.Value) && _actionBlocker.CanInteract(playerEntity, photo);
    }

}

/// <summary>
/// An event sent by the server to the client to tell the client to open a photo window.
/// </summary>
[Serializable, NetSerializable]
public sealed class PhotoViewEvent : EntityEventArgs
{
    public NetEntity PhotoUid;
    public NetEntity CameraUid;
    public Vector2i Size;

    public PhotoViewEvent(NetEntity photoUid, NetEntity cameraUid, Vector2i size)
    {
        PhotoUid = photoUid;
        CameraUid = cameraUid;
        Size = size;
    }
}

/// <summary>
/// An event to tell the server that we have stopped viewing this photo.
/// </summary>
[Serializable, NetSerializable]
public sealed class PhotoStopViewingEvent : EntityEventArgs
{
    /// <summary>
    /// The entity UID of the photo associated with this session.
    /// </summary>
    public NetEntity PhotoUid;

    public PhotoStopViewingEvent(NetEntity photoUid)
    {
        PhotoUid = photoUid;
    }
}
