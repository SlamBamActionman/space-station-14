using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Salvage;

[Prototype]
public sealed partial class SalvageMapPrototype : IPrototype
{
    [ViewVariables] [IdDataField] public string ID { get; private set; } = default!;

    /// <summary>
    /// Relative directory path to the given map, i.e. `Maps/Salvage/template.yml`
    /// </summary>
    [DataField(required: true)] public ResPath MapPath;

    /// <summary>
    /// String that describes the size of the map.
    /// </summary>
    [DataField(required: true)]
    public LocId SizeString;

    /// <summary>
    /// SLAM-NOTE: Display the wreck type I guess. This is probably an enum or something in the future.
    /// </summary>
    [DataField]
    public LocId WreckTypeString = "Default";

    /// <summary>
    /// SLAM-NOTE: Display the wreck name!
    /// </summary>
    [DataField]
    public LocId WreckNameString = "Default";

    /// <summary>
    /// SLAM-NOTE: Job prototype this wreck is tied to, if any.
    /// </summary>
    [DataField]
    public ProtoId<JobPrototype>? JobConnection = null;
}
