using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Photography;

namespace Content.Server.Photography;

public sealed partial class SpriteSaverSystem : SharedSpriteSaverSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<SpriteSaverEvent>(OnSpriteSaving);
    }

    private void OnSpriteSaving(SpriteSaverEvent ev, EntitySessionEventArgs args)
    {
        EntityManager.TryGetEntity(ev.Destination, out EntityUid? destEnt);

        if (destEnt == null)
            return;

        Logger.Debug(destEnt.ToString() ?? "Nothing");

        SpriteSaverComponent comp = EnsureComp<SpriteSaverComponent>(destEnt.Value);
        comp.RsiPath = ev.RsiPath;
        comp.Layers = ev.Layers;
        comp.SnapCardinals = ev.SnapCardinals;
        comp.ScreenLock = ev.ScreenLock;
        comp.Visible = ev.Visible;
        comp.DrawDepth = ev.DrawDepth;

        Dirty(comp);
    }
}
