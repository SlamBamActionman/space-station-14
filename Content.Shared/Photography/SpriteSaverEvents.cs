using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robust.Shared.Serialization;

namespace Content.Shared.Photography;

[Serializable, NetSerializable]
public sealed class SpriteSaverEvent : EntityEventArgs
{
    public NetEntity Destination;

    public string RsiPath;

    public List<LayersStruct> Layers;

    public SpriteSaverEvent(NetEntity destination, string rsiPath, List<LayersStruct> layers)
    {
        Destination = destination;
        RsiPath = rsiPath;
        Layers = layers;
    }

    [Serializable]
    public struct LayersStruct
    {
        public int AnimationFrame;
        public string? ActualRsiPath;
        public string? StateIdName;

        public LayersStruct(int animationFrame, string? stateIdName, string? actualRsiPath)
        {
            AnimationFrame = animationFrame;
            StateIdName = stateIdName;
            ActualRsiPath = actualRsiPath;
        }
    }
}
