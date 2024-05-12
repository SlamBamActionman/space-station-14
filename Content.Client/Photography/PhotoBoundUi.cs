using Content.Client.Eye;
using Content.Shared.SurveillanceCamera;
using Robust.Client.GameObjects;

namespace Content.Client.Photography;

public sealed class PhotoBoundUserInterface : BoundUserInterface
{
    private readonly EyeLerpingSystem _eyeLerpingSystem;

    [ViewVariables]
    private PhotoWindow? _window;

    [ViewVariables]
    private EntityUid? _photoEntity;

    public PhotoBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _eyeLerpingSystem = EntMan.System<EyeLerpingSystem>();
    }

    protected override void Open()
    {
        base.Open();

        _window = new PhotoWindow();

        if (State != null)
        {
            UpdateState(State);
        }

        _window.OpenCentered();

        _window.OnClose += Close;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (_window == null || state is not SurveillanceCameraMonitorUiState cast)
        {
            return;
        }

        var active = EntMan.GetEntity(cast.ActiveCamera);

        if (active == null)
        {
            _window.UpdateState(null);

            if (_photoEntity != null)
            {
                _eyeLerpingSystem.RemoveEye(_photoEntity.Value);
                _photoEntity = null;
            }
        }
        else
        {
            if (_photoEntity == null)
            {
                _eyeLerpingSystem.AddEye(active.Value);
                _photoEntity = active;
            }
            else if (_photoEntity != active)
            {
                _eyeLerpingSystem.RemoveEye(_photoEntity.Value);
                _eyeLerpingSystem.AddEye(active.Value);
                _photoEntity = active;
            }

            if (EntMan.TryGetComponent<EyeComponent>(active, out var eye))
            {
                _window.UpdateState(eye.Eye);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (_photoEntity != null)
        {
            _eyeLerpingSystem.RemoveEye(_photoEntity.Value);
            _photoEntity = null;
        }

        if (disposing)
        {
            _window?.Dispose();
        }
    }
}
