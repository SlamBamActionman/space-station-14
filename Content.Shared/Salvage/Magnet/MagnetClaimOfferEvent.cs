using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.Magnet;

/// <summary>
/// Claim an offer from the magnet UI.
/// </summary>
[Serializable, NetSerializable]
public sealed class MagnetClaimOfferEvent : BoundUserInterfaceMessage
{
    public int Index;
}

/// <summary>
/// SLAM-NOTE: Prototype code. The entire thing should be rewritten.
/// </summary>
[Serializable, NetSerializable]
public sealed class MagnetClaimOfferEventExtra : BoundUserInterfaceMessage
{
    public int Index;
}
