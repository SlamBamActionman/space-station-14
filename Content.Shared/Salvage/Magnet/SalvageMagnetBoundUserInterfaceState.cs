using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.Magnet;

[Serializable, NetSerializable]
public sealed class SalvageMagnetBoundUserInterfaceState : BoundUserInterfaceState
{
    public bool Active;
    public TimeSpan NextOffer;

    public TimeSpan? ClaimTime;

    public TimeSpan Cooldown;
    public TimeSpan Duration;

    public int ActiveSeed;

    public int InitialTileCount;
    public int CurrentTileCount;

    public float ShredderEfficiency;

    public List<int> Offers;

    public int ExtraEntry;

    public SalvageMagnetBoundUserInterfaceState(List<int> offers)
    {
        Offers = offers;
    }
}
