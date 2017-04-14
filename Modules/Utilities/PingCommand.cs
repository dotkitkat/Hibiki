using Discord.Commands;
using System.Threading.Tasks;

namespace Hibiki.Modules.Utilities
{
    [Group, Name("Utilities")]
    public class PingCommand: ModuleBase
    {
        [Command("Ping"), Summary("Pong!")]
        public async Task Invoke()
        {
            await ReplyAsync("Pong!");
        }
    }
}