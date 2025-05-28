using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Shared.Chat.ChatChannels;

public abstract class OutOfSimChannelBase
{
    // The goal of this class is to define properties for out-of-sim channels and provide an entry point to in-sim channels.
    // This includes OOC, Admin and game messages, and a general InGame channel.

    public virtual bool EvaluatePublisher(
        ICommonSession? senderSession,
        Dictionary<Enum, object>? messageProperties = null // Is this necessary if we hardcode?
        )
    {
        return true;
    }

    public virtual Dictionary<HashSet<ICommonSession>,FormattedMessage> CreateConsumerGroups(
        HashSet<ICommonSession> consumerList,
        FormattedMessage inputMessage,
        Dictionary<Enum, object>? messageProperties = null // Is this necessary if we hardcode?
    )
    {
        return new Dictionary<HashSet<ICommonSession>, FormattedMessage>() { { consumerList, inputMessage } };
    }

    public virtual FormattedMessage FormatChannelMessage(
        FormattedMessage message)
    {
        return message;
    }
}
