using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Photography;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpriteSaverComponent : Component
{

    [ViewVariables, AutoNetworkedField]
    public string? RsiPath;

    [ViewVariables, AutoNetworkedField]
    public List<SpriteSaverEvent.LayersStruct>? Layers;

    [ViewVariables, AutoNetworkedField]
    public bool SnapCardinals;

    [ViewVariables, AutoNetworkedField]
    public bool ScreenLock;

    [ViewVariables, AutoNetworkedField]
    public bool Visible;

    [ViewVariables, AutoNetworkedField]
    public int DrawDepth;
}
