using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Photography;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
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
        ApplySpriteData(uid, component.RsiPath, component.Layers, component.SnapCardinals, component.Visible, component.DrawDepth);
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

        if (sprite.BaseRSI == null)
            return;


        Logger.Debug(sprite.BaseRSI.Path.ToString() + " from " + source.ToString() + " to " + dest.ToString());
        List<SpriteSaverEvent.LayersStruct> layerStructs = new List<SpriteSaverEvent.LayersStruct>();


        int i = 0;
        foreach(var layer in sprite.AllLayers)
        {
            var layerStruct = new SpriteSaverEvent.LayersStruct(layer.AnimationFrame, sprite.LayerGetState(i).Name, layer.Rsi?.Path.ToString() ?? null, (byte)layer.DirOffset, layer.Visible);
            Logger.Debug(sprite.LayerGetState(i).Name ?? "It was null!");
            layerStructs.Add(layerStruct);
            i++;
        }


        RaiseNetworkEvent(new SpriteSaverEvent(dest, sprite.BaseRSI.Path.ToString(), layerStructs, sprite.SnapCardinals, sprite.Visible, sprite.DrawDepth));

        var clientEntity = GetEntity(dest);

        if (clientEntity.Valid)
        {
            ApplySpriteData(GetEntity(dest), sprite.BaseRSI!.Path.ToString(), layerStructs, sprite.SnapCardinals, sprite.Visible, sprite.DrawDepth);
        }
    }

    public void ApplySpriteData(EntityUid uid, string? rsiPath, List<SpriteSaverEvent.LayersStruct>? layers, bool snapCardinals, bool visible, int drawDepth)
    {
        if (!uid.Valid)
        {
            Logger.Debug(uid.ToString() + " isn't valid.");
            return;
        }
        EnsureComp<SpriteSaverComponent>(uid);
        

        Logger.Debug("ApplySpriteData ran on uid " + uid.ToString());
        if (rsiPath == null || layers == null || !uid.Valid)
            return;

        Logger.Debug("...with a path! " + rsiPath);
        SpriteComponent spriteComp = EnsureComp<SpriteComponent>(uid);

        if (resourceCache.TryGetResource(rsiPath, out RSIResource? resource))
        {
            spriteComp.BaseRSI = resource.RSI;
        }

        foreach(var layer in layers)
        {
            Logger.Debug("A layer detected: " + layer.StateIdName);
            if (layer.StateIdName != null) 
            {
                Logger.Debug(layer.StateIdName);
                int i = spriteComp.AddLayer(new RSI.StateId(layer.StateIdName));
                if (spriteComp.TryGetLayer(i, out SpriteComponent.Layer? outLayer))
                {
                    outLayer.AnimationFrame = layer.AnimationFrame;
                    outLayer.SetAutoAnimated(false);
                    outLayer.DirOffset = (SpriteComponent.DirectionOffset)layer.DirOffset;
                    outLayer.Visible = layer.Visible;
                }
            }
        }

        spriteComp.SnapCardinals = snapCardinals;
        spriteComp.Visible = visible;
        spriteComp.DrawDepth = drawDepth;
    }
}
