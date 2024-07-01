using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Photography;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using System.Numerics;
using Robust.Shared.Utility;

namespace Content.Client.Photography;

public sealed partial class SpriteSaverSystem : SharedSpriteSaverSystem
{

    [Dependency] private readonly SpriteSystem _spriteSystem = default!;
    [Dependency] private readonly IResourceCache resourceCache = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpriteSaverComponent, ComponentInit>(OnComponentInit);
        SubscribeNetworkEvent<SpriteSaverSourceEvent>(OnSourceGiven);
    }

    private void OnComponentInit(EntityUid uid, SpriteSaverComponent component, ComponentInit args)
    {
        ApplySpriteData(uid, component.RsiPath, component.Layers, component.SnapCardinals, component.ScreenLock, component.Visible, component.DrawDepth, component.Color, new Angle(component.Rotation), component.Scale);
        Logger.Debug(uid.ToString());
    }

    private void OnSourceGiven(SpriteSaverSourceEvent args)
    {
        Logger.Debug("SetupSpriteCopy ran");
        SetupSpriteCopy(GetEntity(args.Source), args.Destination);
    }

    public void SetupSpriteCopy(EntityUid source, NetEntity dest)
    {
        if (!TryComp<SpriteComponent>(source, out var sprite))
            return;


        Logger.Debug("From " + source.ToString() + " to " + dest.ToString());
        List<SpriteSaverEvent.LayersStruct> layerStructs = new List<SpriteSaverEvent.LayersStruct>();


        int i = 0;
        foreach(var layer in sprite.AllLayers)
        {
            string? shaderName = null;
            Vector2 offset = Vector2.Zero;
            if (layer is SpriteComponent.Layer spriteLayer)
            {
                shaderName = spriteLayer.ShaderPrototype;
                offset = spriteLayer.Offset;
            }
            var layerStruct = new SpriteSaverEvent.LayersStruct(layer.AnimationFrame, sprite.LayerGetState(i).Name, layer.ActualRsi?.Path.ToString() ?? null, (byte)layer.DirOffset, offset, layer.Visible, shaderName, layer.Color);
            layerStructs.Add(layerStruct);
            i++;
        }

        string? spriteRSI = null;
        if (sprite.BaseRSI != null)
        {
            spriteRSI = sprite.BaseRSI.Path.ToString();
        }

        RaiseNetworkEvent(new SpriteSaverEvent(dest, spriteRSI, layerStructs, sprite.SnapCardinals, sprite.NoRotation, sprite.Visible, sprite.DrawDepth, sprite.Color, sprite.Rotation.Theta, sprite.Scale));

        var clientEntity = GetEntity(dest);

        if (clientEntity.Valid)
        {
            ApplySpriteData(GetEntity(dest), spriteRSI, layerStructs, sprite.SnapCardinals, sprite.NoRotation, sprite.Visible, sprite.DrawDepth, sprite.Color, sprite.Rotation, sprite.Scale);
        }
    }

    public void ApplySpriteData(EntityUid uid, string? rsiPath, List<SpriteSaverEvent.LayersStruct>? layers, bool snapCardinals, bool screenLock, bool visible, int drawDepth, Color color, Angle rotation, Vector2 scale)
    {
        if (!uid.Valid)
        {
            Logger.Debug(uid.ToString() + " isn't valid.");
            return;
        }
        EnsureComp<SpriteSaverComponent>(uid);
        
        Logger.Debug("ApplySpriteData ran on uid " + uid.ToString());

        SpriteComponent spriteComp = EnsureComp<SpriteComponent>(uid);

        if (rsiPath != null && resourceCache.TryGetResource(rsiPath, out RSIResource? resource))
        {
            spriteComp.BaseRSI = resource.RSI;
        }

        if (layers != null)
        {
            foreach (var layer in layers)
            {
                Logger.Debug("A layer detected: " + layer.StateIdName);
                if (layer.StateIdName != null)
                {
                    Logger.Debug(layer.StateIdName);
                    Logger.Debug(layer.ActualRsiPath ?? "No actual RSI path");
                    int i = spriteComp.AddLayer(new RSI.StateId(layer.StateIdName), layer.ActualRsiPath ?? "");
                    if (spriteComp.TryGetLayer(i, out SpriteComponent.Layer? outLayer))
                    {
                        outLayer.AnimationFrame = layer.AnimationFrame;
                        outLayer.SetAutoAnimated(false);
                        outLayer.DirOffset = (SpriteComponent.DirectionOffset) layer.DirOffset;
                        outLayer.Offset = layer.Offset;
                        outLayer.Visible = layer.Visible;
                        outLayer.Color = layer.Color;
                    }
                    if (layer.ShaderName != null)
                        spriteComp.LayerSetShader(i, layer.ShaderName);
                }
            }
        }

        spriteComp.Color = color;
        spriteComp.SnapCardinals = snapCardinals;
        spriteComp.NoRotation = screenLock;
        spriteComp.Rotation = rotation;
        spriteComp.Visible = visible;
        spriteComp.DrawDepth = drawDepth;
        spriteComp.Scale = scale;
    }
}
