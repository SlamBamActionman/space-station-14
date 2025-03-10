using Content.Client.Administration.Managers;
using Content.Client.Ghost;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Radio;
using Robust.Client.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Chat.Managers;

internal sealed class ChatManager : IChatManager
{
    [Dependency] private readonly IClientConsoleHost _consoleHost = default!;
    [Dependency] private readonly IClientAdminManager _adminMgr = default!;
    [Dependency] private readonly IEntitySystemManager _systems = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private ISawmill _sawmill = default!;

    private static readonly ProtoId<RadioChannelPrototype> DefaultRadioChannel = "Common";

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("chat");
        _sawmill.Level = LogLevel.Info;
    }

    // TODO: Necessary for compatability with how the chatbox system is set up, but should ideally be refactored cleaner.
    public void SendMessage(string text, ChatSelectChannel channel, RadioChannelPrototype? radioChannel)
    {
        // TODO: Get this changed away from being commands.
        var str = text.ToString();
        switch (channel)
        {
            case ChatSelectChannel.Console:
                // run locally
                _consoleHost.ExecuteCommand(text);
                break;

            case ChatSelectChannel.LOOC:
                _consoleHost.ExecuteCommand($"looc \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.OOC:
                _consoleHost.ExecuteCommand($"ooc \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Admin:
                _consoleHost.ExecuteCommand($"asay \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Emotes:
                _consoleHost.ExecuteCommand($"me \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Dead:
                if (_systems.GetEntitySystemOrNull<GhostSystem>() is {IsGhost: true})
                    goto case ChatSelectChannel.Local;

                if (_adminMgr.HasFlag(AdminFlags.Admin))
                    _consoleHost.ExecuteCommand($"dsay \"{CommandParsing.Escape(str)}\"");
                else
                    _sawmill.Warning("Tried to speak on deadchat without being ghost or admin.");
                break;

            // CHAT-TODO: The way this uses a magic string and forces "Radio" + channel id isn't good. Necessary for backwards compatability now, but should be refactored.
            case ChatSelectChannel.Radio:
                radioChannel ??= _prototypeManager.Index<RadioChannelPrototype>(DefaultRadioChannel);
                SendMessage(str, "Radio" + radioChannel.ID);
                break;

            case ChatSelectChannel.Local:
                _consoleHost.ExecuteCommand($"say \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Whisper:
                _consoleHost.ExecuteCommand($"whisper \"{CommandParsing.Escape(str)}\"");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
        }
    }

    public void SendMessage(string text, string channel)
    {
        if (_prototypeManager.TryIndex(channel, out CommunicationChannelPrototype? channelPrototype))
        {
            SendMessage(text, channelPrototype);
        }
    }

    public void SendMessage(string text, CommunicationChannelPrototype channel)
    {
        var str = text.ToString();

        _consoleHost.ExecuteCommand($"chat {channel.ID} \"{CommandParsing.Escape(str)}\"");
    }
}
