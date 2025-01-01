﻿using Content.Shared.Chat.Prototypes;
using Content.Shared.Radio;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Chat.ChatModifiers;

/// <summary>
/// Wraps the message in a [ColorValue="key"] tag.
/// This tag gets replaced with a [color] tag where the "key" attempts to match up to a clientside-selected color.
/// </summary>
[Serializable]
[DataDefinition]
public sealed partial class ColorRadioChatModifier : ChatModifier
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override FormattedMessage ProcessChatModifier(FormattedMessage message, Dictionary<Enum, object> channelParameters)
    {
        IoCManager.InjectDependencies(this);

        if (channelParameters.TryGetValue(DefaultChannelParameters.RadioChannel, out var radio) &&
            _prototypeManager.TryIndex((string)radio, out RadioChannelPrototype? radioPrototype))
        {
            return InsertAroundMessage(message, new MarkupNode("color", new MarkupParameter(radioPrototype.Color), null, false));
        }

        return message;
    }
}