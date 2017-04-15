using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using Hibiki.Common.Extensions;

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
                await Context.Responder().Message("Pong! Latency: " + DiscordSocketClient.Latency + "ms").SendAsync();
            else
            {
                await Context.Responder().Message("Pong!").SendAsync();
            }
        }
    }
}