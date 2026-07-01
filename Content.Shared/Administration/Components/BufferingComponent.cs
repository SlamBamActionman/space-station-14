using Content.Shared.Administration.Systems;

namespace Content.Shared.Administration.Components;

/// <summary>
/// Fakes buffering for an entity, making them stutter and buffer.
/// </summary>
[RegisterComponent]
[Access(typeof(SharedBufferingSystem))]
public sealed partial class BufferingComponent : Component
{
    /// <summary>
    /// Minimum time the buffer will last.
    /// </summary>
    [DataField("minBufferTime")]
    public float MinimumBufferTime = 0.5f;

    /// <summary>
    /// Maximum time the buffer will last.
    /// </summary>
    [DataField("maxBufferTime")]
    public float MaximumBufferTime = 1.5f;

    /// <summary>
    /// Minimum time until the next buffer triggers.
    /// </summary>
    [DataField("minTimeTilNextBuffer")]
    public float MinimumTimeTilNextBuffer = 10.0f;

    /// <summary>
    /// Maximum time until the next buffer triggers.
    /// </summary>
    [DataField("maxTimeTilNextBuffer")]
    public float MaximumTimeTilNextBuffer = 120.0f;

    /// <summary>
    /// Countdown timer until the next buffer triggers.
    /// </summary>
    [DataField]
    public float TimeTilNextBuffer = 15.0f;

    /// <summary>
    /// Icon for the buffering status.
    /// </summary>
    [DataField]
    public EntityUid? BufferingIcon = null;

    /// <summary>
    /// Timer for a running buffer.
    /// </summary>
    [DataField]
    public float BufferingTimer = 0.0f;
}
