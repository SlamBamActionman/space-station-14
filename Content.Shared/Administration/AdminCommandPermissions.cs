using System.IO;
using System.Linq;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared.Administration;

public sealed class AdminCommandPermissions
{
    // Commands executable by anybody.
    public readonly HashSet<string> AnyCommands = new();

    // Commands only executable by admins with one of the given flag masks.
    public readonly Dictionary<string, AdminFlags[]> AdminCommands = new();

    /// <summary>
    /// Reads and loads the admin flag configuration required to execute commands from a given permissions file.
    /// </summary>
    /// <param name="fs">The stream being read.</param>
    public void LoadPermissionsFromStream(Stream fs)
    {
        using var reader = new StreamReader(fs, EncodingHelpers.UTF8);
        var yStream = new YamlStream();
        yStream.Load(reader);
        var root = (YamlSequenceNode) yStream.Documents[0].RootNode;

        foreach (var child in root)
        {
            var map = (YamlMappingNode) child;
            var commands = map.GetNode<YamlSequenceNode>("Commands").Select(p => p.AsString());
            if (map.TryGetNode("Flags", out var flagsNode))
            {
                var flagNames = flagsNode.AsString().Split(",", StringSplitOptions.RemoveEmptyEntries);
                var flags = AdminFlagsHelper.NamesToFlags(flagNames);
                foreach (var cmd in commands)
                {
                    if (!AdminCommands.TryGetValue(cmd, out var exFlags))
                    {
                        AdminCommands.Add(cmd, new[] {flags});
                    }
                    else
                    {
                        var newArr = new AdminFlags[exFlags.Length + 1];
                        exFlags.CopyTo(newArr, 0);
                        newArr[^1] = flags;
                        AdminCommands[cmd] = newArr;
                    }
                }
            }
            else
            {
                AnyCommands.UnionWith(commands);
            }
        }
    }

    /// <summary>
    /// Whether a specific admin can perform a given command.
    /// </summary>
    /// <param name="cmdName">The command to check for.</param>
    /// <param name="admin">The data associated with the admin.</param>
    /// <returns>Returns true if the admin can run the command.</returns>
    public bool CanCommand(string cmdName, AdminData? admin)
    {
        if (AnyCommands.Contains(cmdName))
        {
            // Anybody can use this command.
            return true;
        }

        if (!AdminCommands.TryGetValue(cmdName, out var flagsReq))
        {
            // Server-console only.
            return false;
        }

        if (admin == null)
        {
            // Player isn't an admin.
            return false;
        }

        foreach (var flagReq in flagsReq)
        {
            if (admin.HasFlag(flagReq))
            {
                return true;
            }
        }

        return false;
    }
}
