using System.Linq;
using System.Numerics;
using Content.Client.Message;
using Content.Client.UserInterface.Controls;
using Content.Shared.Roles;
using Content.Shared.Salvage;
using Content.Shared.Salvage.Magnet;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client.Salvage.UI;

public sealed class SalvageMagnetBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IEntitySystemManager _entitySystem = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    private readonly SpriteSystem _spriteSystem;

    private OfferingWindow? _window;

    public SalvageMagnetBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);
        _spriteSystem = _entitySystem.GetEntitySystem<SpriteSystem>();
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindowCenteredLeft<OfferingWindow>();
        _window.Title = Loc.GetString("salvage-magnet-window-title");
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not SalvageMagnetBoundUserInterfaceState current || _window == null)
            return;

        _window.ClearOptions();

        var salvageSystem = _entManager.System<SharedSalvageSystem>();
        _window.NextOffer = current.NextOffer;
        _window.Progression = current.EndTime ?? TimeSpan.Zero;
        _window.Claimed = current.EndTime != null;
        _window.Cooldown = current.Cooldown;
        _window.ProgressionCooldown = current.Duration;

        for (var i = 0; i < current.Offers.Count; i++)
        {
            var seed = current.Offers[i];
            var offer = salvageSystem.GetSalvageOffering(seed);
            var option = new OfferingWindowOption();
            option.MinWidth = 210f;
            option.Disabled = current.EndTime != null;
            option.Claimed = current.ActiveSeed == seed;
            var claimIndex = i;

            option.ClaimPressed += _ =>
            {
                SendMessage(new MagnetClaimOfferEvent
                {
                    Index = claimIndex
                });
            };

            switch (offer)
            {
                case AsteroidOffering asteroid:
                    option.Title = Loc.GetString($"dungeon-config-proto-{asteroid.Id}");
                    var layerKeys = asteroid.MarkerLayers.Keys.ToList();
                    layerKeys.Sort();

                    foreach (var resource in layerKeys)
                    {
                        var count = asteroid.MarkerLayers[resource];

                        var container = new BoxContainer
                        {
                            Orientation = BoxContainer.LayoutOrientation.Horizontal,
                            HorizontalExpand = true,
                        };

                        var resourceLabel = new Label
                        {
                            Text = Loc.GetString("salvage-magnet-resources",
                                ("resource", resource)),
                            HorizontalAlignment = Control.HAlignment.Left,
                        };

                        var countLabel = new Label
                        {
                            Text = Loc.GetString("salvage-magnet-resources-count", ("count", count)),
                            HorizontalAlignment = Control.HAlignment.Right,
                            HorizontalExpand = true,
                        };

                        container.AddChild(resourceLabel);
                        container.AddChild(countLabel);

                        option.AddContent(container);
                    }

                    break;
                case DebrisOffering debris:
                    option.Title = Loc.GetString($"salvage-magnet-debris-{debris.Id}");
                    break;
                case SalvageOffering salvage:
                    option.Title = salvage.SalvageMap.WreckNameString;

                    var salvContainer = new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Horizontal,
                        HorizontalExpand = true,
                    };

                    var sizeLabel = new Label
                    {
                        Text = Loc.GetString("salvage-map-wreck-desc-size"),
                        HorizontalAlignment = Control.HAlignment.Left,
                    };

                    var sizeValueLabel = new RichTextLabel
                    {
                        HorizontalAlignment = Control.HAlignment.Right,
                        HorizontalExpand = true,
                    };
                    sizeValueLabel.SetMarkup(Loc.GetString(salvage.SalvageMap.SizeString));

                    var salvContainerType = new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Horizontal,
                        HorizontalExpand = true,
                    };

                    var typeLabel = new Label
                    {
                        Text = "Wreck Properties",
                        HorizontalAlignment = Control.HAlignment.Left,
                    };

                    var typeValueLabel = new RichTextLabel
                    {
                        Text = salvage.SalvageMap.WreckTypeString,
                        HorizontalAlignment = Control.HAlignment.Right,
                        HorizontalExpand = true,
                    };

                    salvContainer.AddChild(sizeLabel);
                    salvContainer.AddChild(sizeValueLabel);
                    salvContainerType.AddChild(typeLabel);
                    salvContainerType.AddChild(typeValueLabel);

                    option.AddContent(salvContainer);
                    option.AddContent(salvContainerType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _window.AddOption(option);
        }

        // SLAM-NOTE: Prototype code. The entire thing should be rewritten.
        if (current.ExtraEntry != 0)
        {
            var seedExtra = current.ExtraEntry;
            var mapExtra = salvageSystem.TestGetSalvageMapPrototype(seedExtra);
            var offerExtra = new SalvageOffering();
            offerExtra.SalvageMap = mapExtra;
            var optionExtra = new OfferingWindowOption();
            optionExtra.MinWidth = 210f;
            optionExtra.Disabled = current.EndTime != null;
            optionExtra.Claimed = current.ActiveSeed == seedExtra;

            optionExtra.ClaimPressed += _ =>
            {
                SendMessage(new MagnetClaimOfferEventExtra
                {
                    Index = seedExtra
                });
            };

            switch (offerExtra)
            {
                case SalvageOffering salvage:
                    optionExtra.Title = salvage.SalvageMap.WreckNameString;

                    var salvContainer = new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Horizontal,
                        HorizontalExpand = true,
                    };

                    var sizeLabel = new Label
                    {
                        Text = Loc.GetString("salvage-map-wreck-desc-size"),
                        HorizontalAlignment = Control.HAlignment.Left,
                    };

                    var sizeValueLabel = new RichTextLabel
                    {
                        HorizontalAlignment = Control.HAlignment.Right,
                        HorizontalExpand = true,
                    };
                    sizeValueLabel.SetMarkup(Loc.GetString(salvage.SalvageMap.SizeString));

                    var salvContainerType = new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Horizontal,
                        HorizontalExpand = true,
                    };

                    var typeLabel = new Label
                    {
                        Text = "Properties:",
                        HorizontalAlignment = Control.HAlignment.Left,
                    };

                    var typeValueLabel = new RichTextLabel
                    {
                        Text = salvage.SalvageMap.WreckTypeString,
                        HorizontalAlignment = Control.HAlignment.Right,
                        HorizontalExpand = true,
                    };

                    var salvContainerJob = new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Horizontal,
                        HorizontalExpand = true,
                    };

                    var jobLabel = new Label
                    {
                        Text = "Faction: ",
                        HorizontalAlignment = Control.HAlignment.Left,
                    };
                    salvContainerJob.AddChild(jobLabel);

                    if (_proto.TryIndex<JobPrototype>(salvage.SalvageMap.JobConnection, out var job))
                    {
                        if (_proto.TryIndex<JobIconPrototype>(job.Icon, out var jobIcon))
                        {
                            var icon = new TextureRect()
                            {
                                TextureScale = new Vector2(2, 2),
                                VerticalAlignment = Control.VAlignment.Center,
                                HorizontalAlignment = Control.HAlignment.Right,
                                HorizontalExpand = true,
                                Texture = _spriteSystem.Frame0(jobIcon.Icon),
                                Margin = new Thickness(0, 0, 4, 0)
                            };

                            salvContainerJob.AddChild(icon);
                        }
                    }

                    salvContainer.AddChild(sizeLabel);
                    salvContainer.AddChild(sizeValueLabel);
                    salvContainerType.AddChild(typeLabel);
                    salvContainerType.AddChild(typeValueLabel);

                    optionExtra.AddContent(salvContainer);
                    optionExtra.AddContent(salvContainerType);
                    optionExtra.AddContent(salvContainerJob);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _window.AddOption(optionExtra);
        }
        else
        {
            var placeholder = new Placeholder();
            placeholder.PlaceholderText = "Shuttle Manifest slot";
            _window.AddPlaceholderOption(placeholder);
        }
    }
}
