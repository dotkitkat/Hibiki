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
            var Embed = new EmbedBuilder { Title = "You can use the following commands." };

            foreach (var Group in CommandGroups)
            {
                var CommandsAvailable = Group.Select(commandInfo => commandInfo.Module.IsSubmodule
                        ? $"`{commandInfo.Module.Name}*`"
                        : $"`{commandInfo.Name}`")
                    .ToList();

                Builder.AppendLine($"**{Group.Key}**: {string.Join(" ", CommandsAvailable.Distinct())}");
            }
            Builder.AppendLine(
                $"\nYou can use `{await PrefixManager.GetPrefixAsync(_Mongo, Context.Guild)}help <command>` for more information on that command.");

            Embed.Description = Builder.ToString();

            await Context.Channel.SendMessageAsync(string.Empty, embed: Embed);
        }

        [Command("Help"), Summary("Gets specific info for a command."), Hidden]
        public async Task InvokeSpecific(string command)
        {
            await ReplyAsync("This feature is currently in-development.");
        }
    }
}