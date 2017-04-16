using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hibiki.Common.Extensions;
using Hibiki.Common.Permissions;

namespace Hibiki.Modules.Bot
{
    [Name("Bot")]
    public class BotInfo: ModuleBase
    {
        [Command("BotInfo"), Summary("Gets information about the bot.")]
        [RequireOwner]
        public async Task Info()
        {
            var Application = await Context.Client.GetApplicationInfoAsync();
            var DiscordSocketClient = Context.Client as DiscordSocketClient;
            var Embed = Common.Embeds.Embed.Info();
            Embed.Title = "Hibiki Information";
            if (DiscordSocketClient != null)
            {
                Embed.AddInlineField("Author", $"{Application.Owner.Username} (ID {Application.Owner.Id})");
                Embed.AddInlineField("Library", $"Discord.NET ({DiscordConfig.Version})");
                Embed.AddInlineField("Version", "RC-1.0");
                Embed.AddInlineField("Uptime", GetUptime());
                Embed.AddInlineField("Heap Size", GetHeapSize() + "mb");
                Embed.AddInlineField("Guilds", DiscordSocketClient.Guilds.Count);
                Embed.AddInlineField("Channels", DiscordSocketClient.Guilds.Sum(guild => guild.Channels.Count));
                Embed.AddInlineField("Users", DiscordSocketClient.Guilds.Sum(guild => guild.Users.Count));
                await Context.EmbedAsync(Embed);
            }
        }

        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");

        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString(CultureInfo.InvariantCulture);
    }
}