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
/// Indicates the current magnet claim has been ended.
/// </summary>
[Serializable, NetSerializable]
public sealed class MagnetClaimEndedEvent : BoundUserInterfaceMessage
{

}


/// <summary>
/// SLAM-NOTE: Prototype code. The entire thing should be rewritten.
/// </summary>
[Serializable, NetSerializable]
public sealed class MagnetClaimOfferEventExtra : BoundUserInterfaceMessage
{
    public int Index;
}
