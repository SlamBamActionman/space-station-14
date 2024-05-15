using System.Numerics;

namespace Content.Server.Photography;

/// <summary>
/// A component that makes an object viewable as a photo.
/// </summary>
[RegisterComponent, Access(typeof(PhotoSystem))]
public sealed partial class PhotoSessionComponent : Component
{
    /// <summary>
    /// The size of the viewport being opened. Must match the board dimensions otherwise you'll get the space parallax (unless that's what you want).
    /// </summary>
    [DataField]
    public Vector2i Size { get; private set; } = (300, 300);

    /// <summary>
    /// The zoom of the viewport camera.
    /// </summary>
    [DataField]
    public Vector2 CameraZoom { get; private set; } = Vector2.One;

    /// <summary>
    /// Angle of the camera when photo was taken.
    /// </summary>
    [DataField]
    public Angle CameraAngle { get; set; } = Angle.Zero;

    /// <summary>
    /// The specific session of this tabletop.
    /// </summary>
    [ViewVariables]
    public PhotoSession? Session { get; set; } = null;
}
