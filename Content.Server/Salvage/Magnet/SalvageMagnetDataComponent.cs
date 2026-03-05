using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Salvage.Magnet;

/// <summary>
/// Added to the station to hold salvage magnet data.
/// </summary>
[RegisterComponent]
public sealed partial class SalvageMagnetDataComponent : Component
{
    // May be multiple due to splitting.

    /// <summary>
    /// Entities currently magnetised.
    /// </summary>
    [DataField]
    public List<EntityUid>? ActiveEntities;

    /// <summary>
    /// If the magnet is currently active.
    /// </summary>
    [DataField]
    public bool Active;

    /// <summary>
    /// When the magnet had an offer claimed.
    /// </summary>
    [DataField(customTypeSerializer:typeof(TimeOffsetSerializer))]
    public TimeSpan? ClaimTime;

    [DataField(customTypeSerializer:typeof(TimeOffsetSerializer))]
    public TimeSpan NextOffer;

    /// <summary>
    /// How long salvage will be active for before despawning.
    /// </summary>
    [DataField]
    public TimeSpan ActiveTime = TimeSpan.FromMinutes(6);

    /// <summary>
    /// Cooldown between offerings after one ends.
    /// </summary>
    [DataField]
    public TimeSpan OfferCooldown = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Seeds currently offered
    /// </summary>
    [DataField]
    public List<int> Offered = new();

    [DataField]
    public int OfferCount = 2;

    [DataField]
    public int ActiveSeed;

    /// <summary>
    /// The number of tiles the salvage magnet has pulled for the active seed when spawning.
    /// </summary>
    [DataField]
    public int InitialTileCount;

    /// <summary>
    /// The number of tiles currently remaining for the salvage magnet.
    /// </summary>
    [DataField]
    public int CurrentTileCount;

    /// <summary>
    /// The number of valuable entities that should be extracted from the salvage pull.
    /// </summary>
    [DataField]
    public int InitialValuablesCount;

    /// <summary>
    /// The number of valuable entities that got incorrectly processed in the salvage pull.
    /// </summary>
    [DataField]
    public int IncorrectlyProcessedValuablesCount;

    /// <summary>
    /// Final countdown announcement.
    /// </summary>
    [DataField]
    public bool Announced;
}
