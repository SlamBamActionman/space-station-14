using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Photography;

[RegisterComponent, NetworkedComponent]
public sealed partial class SpriteSaverComponent : Component
{

    [ViewVariables]
    public string? RsiPath;

    [ViewVariables]
    public List<SpriteSaverEvent.LayersStruct>? Layers;
}
