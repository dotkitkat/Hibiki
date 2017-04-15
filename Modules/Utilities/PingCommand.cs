using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Hibiki.Modules.Utilities
{
    [Group, Name("Utilities")]
    public class PingCommand: ModuleBase
    {
        [Command("Ping"), Summary("Pong!")]
        public async Task Invoke()
        {
            var DiscordSocketClient = Context.Client as DiscordSocketClient;
            if (DiscordSocketClient != null)
                await ReplyAsync("Pong! Latency: " + DiscordSocketClient.Latency + "ms");
            else
            {
                await ReplyAsync("Pong!");
            }
        }
    }
}