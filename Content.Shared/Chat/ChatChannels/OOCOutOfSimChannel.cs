using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Shared.Chat.ChatChannels;

public sealed class OOCOutOfSimChannel : OutOfSimChannelBase
{
    public override bool EvaluatePublisher(
        ICommonSession? senderSession,
        Dictionary<Enum, object>? messageProperties = null // Is this necessary if we hardcode?
        )
    {
        return true;
    }

    public override Dictionary<HashSet<ICommonSession>,FormattedMessage> CreateConsumerGroups(
        HashSet<ICommonSession> consumerList,
        FormattedMessage inputMessage,
        Dictionary<Enum, object>? messageProperties = null // Is this necessary if we hardcode?
    )
    {
        return new Dictionary<HashSet<ICommonSession>, FormattedMessage>() { { consumerList, inputMessage } };
    }

    public override FormattedMessage FormatChannelMessage(
        FormattedMessage message)
    {
        return message;
    }
}
