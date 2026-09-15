using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.EntityEffects.Effects.Transform;

/// <summary>
/// Plays a single instance of an audio clip within PVS when triggered.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T, TEffect}"/>
public sealed partial class PlayAudioOnEntityEntityEffectSystem : EntityEffectSystem<TransformComponent, PlayLocalAudio>
{
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private INetManager _net = default!;
    [Dependency] private IGameTiming _timing = default!;

    protected override void Effect(Entity<TransformComponent> entity, ref EntityEffectEvent<PlayLocalAudio> args)
    {
        if (args.Effect.Predicted)
        {
            // TODO: This should ideally be done with PlayLocal, but that method does not support coordinates, only entity. This is a problem if the entity gets deleted.
            if (_net.IsServer || !_timing.IsFirstTimePredicted)
                return;

            if (args.Effect.FollowEntity)
                _audio.PlayLocal(args.Effect.Sound, entity.Owner, args.User);
            else
                _audio.PlayPvs(args.Effect.Sound, Transform(entity.Owner).Coordinates);
        }
        else
        {
            if (!_net.IsServer)
                return;

            if (args.Effect.FollowEntity)
                _audio.PlayPvs(args.Effect.Sound, entity.Owner);
            else
                _audio.PlayPvs(args.Effect.Sound, Transform(entity.Owner).Coordinates);
        }
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class PlayLocalAudio : EntityEffectBase<PlayLocalAudio>
{
    /// <summary>
    /// Sound to play when triggered.
    /// </summary>
    [DataField(required: true)]
    public SoundSpecifier Sound { get; set; } = default!;

    /// <summary>
    /// If true, the sound will follow the entity. If false, it will instead target the coordinates.
    /// </summary>
    [DataField]
    public bool FollowEntity = true;

    /// <summary>
    /// If true, the sound will only play locally on the user's client. If false, the server will handle sending the sound.
    /// </summary>
    [DataField]
    public bool Predicted = true;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) => null;
}
