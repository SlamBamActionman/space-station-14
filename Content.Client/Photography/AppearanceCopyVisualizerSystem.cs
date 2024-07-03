using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Photography;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Content.Shared.Tag;
using Content.Client.Markers;

namespace Content.Client.Photography;

public sealed class AppearanceCopyVisualizerSystem : EntitySystem
{

    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly MarkerSystem _markerSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AppearanceCopyComponent, ComponentHandleState>(OnAppearanceCopyHandleState);

    }

    public void OnAppearanceCopyHandleState(EntityUid uid, AppearanceCopyComponent comp, ref ComponentHandleState args)
    {
        if (args.Current is not AppearanceCopyComponentState state) return;

        if (comp.PrototypeId == state.PrototypeId) return;

        comp.PrototypeId = state.PrototypeId;
        var appearanceComp = EnsureComp<AppearanceComponent>(uid);

        Logger.Debug(state.PrototypeId);

        var entProto = _protoMan.Index<EntityPrototype>(state.PrototypeId);

        if (entProto.TryGetComponent(out SpriteComponent? sourceSpriteComp, _componentFactory))
        {
            var spriteComp = EnsureComp<SpriteComponent>(uid);
            spriteComp.CopyFrom(sourceSpriteComp);
        }


        var filteredComponents = new ComponentRegistry(entProto.Components.Where(kvp => (
        kvp.Key.Contains("Visuals") ||
        kvp.Key.Contains("Visualizer") ||
        kvp.Key == "IconSmooth" ||
        kvp.Key == "RandomSprite" ||
        kvp.Key == "HumanoidAppearance" ||
        kvp.Key == "Inventory" ||
        kvp.Key == "VendingMachine" ||
        kvp.Key == "Pda" ||
        kvp.Key == "Marker"
        )).ToDictionary());

        _markerSystem.MarkersVisible = false;

        EntityManager.AddComponents(uid, filteredComponents);

        Dirty(uid, appearanceComp);
    }

    /*
    protected override void OnAppearanceChange(EntityUid uid, AppearanceCopyComponent comp, ref AppearanceChangeEvent args)
    {

        base.OnAppearanceChange(uid, comp, ref args);

        if (!AppearanceSystem.TryGetData<string>(uid, AppearanceCopyVisuals.Prototype, out var prototype, args.Component))
            return;

        Logger.Debug(prototype);

        var entProto = _protoMan.Index<EntityPrototype>(prototype);

        if (entProto.TryGetComponent(out SpriteComponent? sourceSpriteComp, _componentFactory))
        {
            var spriteComp = EnsureComp<SpriteComponent>(uid);
            spriteComp.CopyFrom(sourceSpriteComp);
        }

        var filteredComponents = new ComponentRegistry(entProto.Components.Where(kvp => kvp.Key.Contains("Visuals")).ToDictionary<string, EntityPrototype.ComponentRegistryEntry>());

        AppearanceSystem.QueueUpdate(uid, EnsureComp<AppearanceComponent>(uid));

        var visualsComponent = filteredComponents;
        EntityManager.AddComponents(uid, visualsComponent);
    }*/
}
