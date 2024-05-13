using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Photography;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Utility;

namespace Content.Client.Photography;

public sealed partial class SpriteSaverSystem : SharedSpriteSaverSystem
{

    [Dependency] private readonly SpriteSystem _spriteSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpriteSaverComponent, ComponentInit>(OnComponentInit);
        SubscribeNetworkEvent<SpriteSaverSourceEvent>(OnSourceGiven);
    }

    private void OnComponentInit(EntityUid uid, SpriteSaverComponent component, ComponentInit args)
    {
        ApplySpriteData(uid, component.RsiPath, component.Layers);
    }

    private void OnSourceGiven(SpriteSaverSourceEvent args)
    {
        Logger.Debug("SetupSpriteCopy ran");
        SetupSpriteCopy(GetEntity(args.Source), GetEntity(args.Destination));
    }

    public void SetupSpriteCopy(EntityUid source, EntityUid dest)
    {
        if (!TryComp<SpriteComponent>(source, out var sprite))
            return;

        List<SpriteSaverEvent.LayersStruct> layerStructs = new List<SpriteSaverEvent.LayersStruct>();

        foreach(var layer in sprite.AllLayers)
        {
            var layerStruct = new SpriteSaverEvent.LayersStruct(layer.AnimationFrame, layer.RsiState.Name ?? null, layer.Rsi?.Path.ToString() ?? null);
            layerStructs.Add(layerStruct);
        }

        RaiseNetworkEvent(new SpriteSaverEvent(EntityManager.GetNetEntity(dest), sprite.BaseRSI!.Path.ToString(), layerStructs));

        ApplySpriteData(dest, sprite.BaseRSI!.Path.ToString(), layerStructs);
    }

    public void ApplySpriteData(EntityUid uid, string? rsiPath, List<SpriteSaverEvent.LayersStruct>? layers)
    {

        Logger.Debug("ApplySpriteData ran");
        if (rsiPath == null || layers == null)
            return;

        Logger.Debug("...with a path!");
        SpriteComponent spriteComp = EnsureComp<SpriteComponent>(uid);
        spriteComp.BaseRSI = new RSI(new Vector2i(32, 32), new ResPath(rsiPath));
        foreach(SpriteSaverEvent.LayersStruct layer in layers)
        {
            spriteComp.AddLayer(new RSI.StateId(layer.StateIdName), layer.ActualRsiPath ?? "");
        }
    }
}
