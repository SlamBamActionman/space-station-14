using System.Numerics;
using Content.Client.Tabletop.UI;
using Content.Client.Viewport;
using Content.Shared.Photography;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.Photography;

[UsedImplicitly]
public sealed class PhotoSytem : SharedPhotoSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly EyeSystem _eye = default!;

    private DefaultWindow? _window; // Current open tabletop window (only allow one at a time)
    private EntityUid? _photo; // The table entity of the currently open game session

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<PhotoViewEvent>(OnPhotoView);
        SubscribeNetworkEvent<QueryPhotoRotationEvent>(OnRotationQuery);
    }

    public override void FrameUpdate(float frameTime)
    {
        if (_window == null)
            return;

        // If there is no player entity, return
        if (_playerManager.LocalEntity is not { } playerEntity)
            return;

        if (!CanSeePhoto(playerEntity, _photo))
        {
            _window?.Close();
            return;
        }
    }

    /// <summary>
    /// Runs when the player presses the "Play Game" verb on a tabletop game.
    /// Opens a viewport where they can then play the game.
    /// </summary>
    private void OnPhotoView(PhotoViewEvent msg)
    {
        // Close the currently opened window, if it exists
        _window?.Close();

        _photo = GetEntity(msg.PhotoUid);

        // Get the camera entity that the server has created for us
        var camera = GetEntity(msg.CameraUid);

        if (!EntityManager.TryGetComponent<EyeComponent>(camera, out var eyeComponent))
        {
            // If there is no eye, print error and do not open any window
            Log.Error("Camera entity does not have eye component!");
            return;
        }
        else
        {
            _eye.SetRotation(camera, msg.CameraAngle);
            Logger.Debug(msg.CameraAngle.ToString());
        }

        // Create a window to contain the viewport
        _window = new PhotoWindow(eyeComponent.Eye, (msg.Size.X, msg.Size.Y))
        {
            MinWidth = 500,
            MinHeight = 436
        };

        _window.OnClose += OnWindowClose;
    }

    private void OnWindowClose()
    {
        if (_photo != null)
        {
            RaiseNetworkEvent(new PhotoStopViewingEvent(GetNetEntity(_photo.Value)));
        }

        _window = null;
    }

    private void OnRotationQuery(QueryPhotoRotationEvent msg)
    {
        Angle angle = Angle.Zero;
        if (TryComp(_playerManager.LocalEntity, out EyeComponent? eyeComp)) {
            angle = eyeComp.Rotation;
        }
        RaiseNetworkEvent(new ProvidePhotoRotationEvent(angle, msg.PhotoUid));
    }
}
