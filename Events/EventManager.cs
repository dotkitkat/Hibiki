using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Hibiki.Events
{
    public class EventManager
    {
        public async Task RegisterEventsAsync(IDependencyMap map)
        {
            await Task.Run(() =>
            {
                var Client = map.Get<DiscordSocketClient>();
                Client.Ready += ReadyEvent.Ready;
            });
        }
    }
}