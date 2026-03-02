namespace Content.Shared.Salvage.Magnet;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class SalvageMagnetComponent : Component
{
    /// <summary>
    /// The max distance at which the magnet will pull in wrecks.
    /// Scales from 50% to 100%.
    /// </summary>
    [DataField]
    public float MagnetSpawnDistance = 64f;

    /// <summary>
    /// How far offset to either side will the magnet wreck spawn.
    /// </summary>
    [DataField]
    public float LateralOffset = 16f;

    /// <summary>
    /// SLAM-TODO: This is just a very temporary hack. 0 is no extra, 1-6 are the other ships
    /// </summary>
    [DataField, AutoNetworkedField]
    public int ExtraEntry = 0;
}
