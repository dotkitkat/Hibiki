using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using Hibiki.Common.Extensions;
using Hibiki.Common.Language;
using MongoDB.Driver;

namespace Hibiki.Modules.Utilities
{
    [Group, Name("Utilities")]
    public class PingCommand: ModuleBase
    {
        private readonly MongoClient _Mongo;

        public PingCommand(IDependencyMap map)
        {
            _Mongo = map.Get<MongoClient>();
        }

        [Command("Ping"), Summary("Pong!")]
        public async Task Invoke()
        {
            var DiscordSocketClient = Context.Client as DiscordSocketClient;
            if (DiscordSocketClient != null)

                await Context.Responder()
                    .Message((await LanguageManager.GetStringAsync(_Mongo, "ping_latency", Context.Guild)).Replace(
                        "{{latency}}", DiscordSocketClient.Latency.ToString()))
                    .ReplyAsync();
            else
            {
                await Context.Responder().Message(await LanguageManager.GetStringAsync(_Mongo, "ping_no_latency", Context.Guild)).ReplyAsync();
            }
        }
    }
}