using Robust.Shared.GameStates;

namespace Content.Shared.Salvage.Magnet;

/// <summary>
/// When an entity with this component is inserted into the Salvage magnet, it will be added as a Salvage offering.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SalvageOfferingProviderComponent : Component
{
    [DataField]
    public bool Available;

    //[DataField]
    //public ISalvageMagnetOffering Offering = default!;

    // SLAM-TODO: This is a temporary hack. Really the entire salvage magnet system should be rewritten to support this new usecase.
    // 1-6 are the test ships.
    [DataField]
    public int Offering = 4;
}
