using System;
using System.Threading.Tasks;
using Discord.Commands;
using Hibiki.Common.Attributes;
using System.Text;
using Discord;
using System.Collections.Generic;
using System.Linq;
using Hibiki;
using Hibiki.Common.Extensions;
using Hibiki.Database;
using MongoDB.Driver;

namespace Hibiki.Modules
{
    [Name("Meta")]
    public class HelpCommand: ModuleBase
    {
        private readonly CommandService _Commands;
        private readonly IDependencyMap _Map;
        private readonly MongoClient _Mongo;

        public HelpCommand(CommandService service, IDependencyMap map)
        {
            _Commands = service ?? throw new ArgumentNullException(nameof(service));
            _Map = map;
            _Mongo = map.Get<MongoClient>();
        }

        [Command("Help"), Summary("Shows this message.")]
        public async Task InvokeDefault()
        {
            var CommandGroups = (await _Commands.Commands.CheckConditionsAsync(Context, _Map))
                .Where(c => !c.Preconditions.Any(p => p is HiddenAttribute))
                .GroupBy(c => c.Module.IsSubmodule ? c.Module.Parent.Name : c.Module.Name);

            var Builder = new StringBuilder();
            var Embed = Common.Embeds.Embed.Info();
            Embed.Title = "You can use the following commands.";

            foreach (var Group in CommandGroups)
            {
                var CommandsAvailable = Group.Select(commandInfo => commandInfo.Module.IsSubmodule
                        ? $"`{commandInfo.Module.Name}*`"
                        : $"`{commandInfo.Name}`")
                    .ToList();

                Builder.AppendLine($"**{Group.Key}**: {string.Join(" ", CommandsAvailable.Distinct())}");
                //Embed.AddInlineField(Group.Key, string.Join(" ", CommandsAvailable.Distinct()));
            }
            Builder.AppendLine(
                $"\nYou can use `{await PrefixManager.GetPrefixAsync(_Mongo, Context.Guild)}help <command>` for more information on that command.");

            Embed.Description = Builder.ToString();

            await Logger.DebugAsync("Breakpoint-3");
            await Logger.DebugAsync(Builder.ToString());

            await Context.Channel.SendMessageAsync(string.Empty, embed: Embed);
        }

        [Command("Help"), Summary("Gets specific info for a command."), Hidden]
        public async Task InvokeSpecific(string command)
        {
            var Commands = (await _Commands.Commands.CheckConditionsAsync(Context, _Map)).Where(
                c => (c.Aliases.FirstOrDefault().Equals(command, StringComparison.OrdinalIgnoreCase)) ||
                     (c.Module.IsSubmodule && c.Module.Aliases.FirstOrDefault()
                          .Equals(command, StringComparison.OrdinalIgnoreCase)) &&
                     !c.Preconditions.Any(p => p is HiddenAttribute));

            var Embeds = new List<EmbedBuilder>();
            var Builder = new StringBuilder();
            var CommandInfos = Commands as IList<CommandInfo> ?? Commands.ToList();
            if (CommandInfos.Any())
            {
                Builder.AppendLine($"Found {CommandInfos.Count} {(CommandInfos.Count > 1 ? "entries" : "entry")} for `{command}`");
                foreach (var Command in CommandInfos)
                {
                    var Embed = Common.Embeds.Embed.Info();
                    Embed.Title = "Command " + Command.Name;
                    Embed.AddInlineField("Usage",
                        $"\t{await PrefixManager.GetPrefixAsync(_Mongo, Context.Guild)}{(Command.Module.IsSubmodule ? $"{Command.Module.Name} " : "")}{Command.Name} " +
                        string.Join(" ", Command.Parameters.Select(FormatParam)).Replace("`", ""));
                    Embed.AddInlineField("Summary", $"\t{Command.Summary ?? "No summary"}");
                    Embeds.Add(Embed);
                }
                foreach (var Embed in Embeds)
                {
                    await Context.Channel.SendMessageAsync(string.Empty, embed: Embed);
                }
            }
            else
            {
                await ReplyAsync($"Couldn't find any command matching `{command}`.");
            }
        }

        private static string FormatParam(ParameterInfo parameter)
        {
            var sb = new StringBuilder();
            if (parameter.IsMultiple)
            {
                sb.Append($"`[({parameter.Type.Name}): {parameter.Name}...]`");
            }
            else if (parameter.IsRemainder)
            {
                sb.Append($"`<({parameter.Type.Name}): {parameter.Name}...>`");
            }
            else if (parameter.IsOptional)
            {
                sb.Append($"`[({parameter.Type.Name}): {parameter.Name}]`");
            }
            else
            {
                sb.Append($"`<({parameter.Type.Name}): {parameter.Name}>`");
            }

            if (!string.IsNullOrWhiteSpace(parameter.Summary))
            {
                sb.Append($" ({parameter.Summary})");
            }
            return sb.ToString();
        }
    }
}