using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robust.Shared.Serialization;
using Robust.Shared.Player;

namespace Content.Shared.Photography;

public abstract class SharedSpriteSaverSystem : EntitySystem
{

    public void SetSourceEntity(EntityUid dest, EntityUid source, ICommonSession player)
    {
        Logger.Debug("SetSourceEntity ran");
        RaiseNetworkEvent(new SpriteSaverSourceEvent(EntityManager.GetNetEntity(source), EntityManager.GetNetEntity(dest)), player.Channel);
    }

}

[Serializable, NetSerializable]
public sealed class SpriteSaverSourceEvent : EntityEventArgs
{
    public NetEntity Source { get; private set; }
    public NetEntity Destination { get; private set; }

    public SpriteSaverSourceEvent(NetEntity source, NetEntity destination)
    {
        Source = source;
        Destination = destination;
    }
}
