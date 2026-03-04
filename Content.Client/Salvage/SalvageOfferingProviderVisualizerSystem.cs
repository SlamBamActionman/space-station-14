using Content.Shared.Salvage.Magnet;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.Salvage;

public sealed class SalvageOfferingProviderVisualizerSystem : VisualizerSystem<SalvageOfferingProviderComponent>
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private static readonly ProtoId<JobIconPrototype> UnknownIcon = "JobIconUnknown";

    protected override void OnAppearanceChange(EntityUid uid, SalvageOfferingProviderComponent component, ref AppearanceChangeEvent args)
    {
        Logger.Debug("1");

        if (args.Sprite == null)
            return;
        Logger.Debug("2");

        _appearance.TryGetData(uid, SalvageOfferingProviderVisuals.JobIcon, out string job, args.Component);

        Logger.Debug("Weh: " + job);


        if (string.IsNullOrEmpty(job))
            job = UnknownIcon;

        if (!_prototype.TryIndex<JobIconPrototype>(job, out var icon))
        {
            SpriteSystem.LayerSetTexture((uid, args.Sprite), SalvageOfferingProviderVisualLayers.JobStamp, SpriteSystem.Frame0(_prototype.Index(UnknownIcon).Icon));
            return;
        }


        SpriteSystem.LayerSetTexture((uid, args.Sprite), SalvageOfferingProviderVisualLayers.JobStamp, SpriteSystem.Frame0(icon.Icon));
    }
}

public enum SalvageOfferingProviderVisualLayers : byte
{
    JobStamp,
    Lock,
}
