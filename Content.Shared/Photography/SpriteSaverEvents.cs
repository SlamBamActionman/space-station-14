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

    public bool SnapCardinals;

    public bool ScreenLock;

    public bool Visible;

    public int DrawDepth;

    public SpriteSaverEvent(NetEntity destination, string rsiPath, List<LayersStruct> layers, bool snapCardinals, bool screenLock, bool visible, int drawDepth)
    {
        Destination = destination;
        RsiPath = rsiPath;
        Layers = layers;
        SnapCardinals = snapCardinals;
        ScreenLock = screenLock;
        Visible = visible;
        DrawDepth = drawDepth;
    }

    [Serializable]
    public struct LayersStruct
    {
        public int AnimationFrame;
        public string? ActualRsiPath;
        public string? StateIdName;
        public byte DirOffset;
        public bool Visible;

        public LayersStruct(int animationFrame, string? stateIdName, string? actualRsiPath, byte dirOffset, bool visible)
        {
            AnimationFrame = animationFrame;
            StateIdName = stateIdName;
            ActualRsiPath = actualRsiPath;
            DirOffset = dirOffset;
            Visible = visible;
        }
    }

}
