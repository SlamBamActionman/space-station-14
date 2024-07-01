using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Robust.Shared.Serialization;

namespace Content.Shared.Photography;

[Serializable, NetSerializable]
public sealed class SpriteSaverEvent : EntityEventArgs
{
    public NetEntity Destination;

    public string? RsiPath;

    public List<LayersStruct> Layers;

    public bool SnapCardinals;

    public bool ScreenLock;

    public bool Visible;

    public int DrawDepth;

    public Color Color;

    public double Rotation;

    public Vector2 Scale;

    public SpriteSaverEvent(NetEntity destination, string? rsiPath, List<LayersStruct> layers, bool snapCardinals, bool screenLock, bool visible, int drawDepth, Color color, double rotation, Vector2 scale)
    {
        Destination = destination;
        RsiPath = rsiPath;
        Layers = layers;
        SnapCardinals = snapCardinals;
        ScreenLock = screenLock;
        Visible = visible;
        DrawDepth = drawDepth;
        Color = color;
        Rotation = rotation;
        Scale = scale;
    }

    [Serializable]
    public struct LayersStruct
    {
        public int AnimationFrame;
        public string? ActualRsiPath;
        public string? StateIdName;
        public byte DirOffset;
        public Vector2 Offset;
        public bool Visible;
        public string? ShaderName;
        public Color Color;

        public LayersStruct(int animationFrame, string? stateIdName, string? actualRsiPath, byte dirOffset, Vector2 offset, bool visible, string? shaderName, Color color)
        {
            AnimationFrame = animationFrame;
            StateIdName = stateIdName;
            ActualRsiPath = actualRsiPath;
            DirOffset = dirOffset;
            Offset = offset;
            Visible = visible;
            ShaderName = shaderName;
            Color = color;
        }
    }

}
