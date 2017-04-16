using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Hibiki.Common.Extensions;

namespace Hibiki.Modules.Bot
{
    [Name("Bot")]
    public class Status: ModuleBase
    {
        [Command("Status"), Summary("Gets my status.")]
        public async Task StatusCommand()
        {
            var Status = new StringBuilder();
            Status.AppendLine(Context.Responder().Message("Online!").ToString());
            Status.AppendLine(Context.Responder()
                .Emoji(":information_source:")
                .Message(Context.Client is DiscordSocketClient ? "Gateway latency: " + ((DiscordSocketClient) Context.Client).Latency + "ms" : "")
                .ToString());
            var Embed = Common.Embeds.Embed.Info();
            Embed.Title = "Status";
            Embed.Description = Status.ToString();
            await Context.EmbedAsync(Embed);
        }
    }
}