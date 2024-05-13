namespace Content.Server.Photography;

/// <summary>
///     Component for marking an entity as currently playing a tabletop.
/// </summary>
[RegisterComponent, Access(typeof(PhotoSystem))]
public sealed partial class PhotoViewerComponent : Component
{
    [DataField]
    public EntityUid Photo { get; set; } = EntityUid.Invalid;
}
