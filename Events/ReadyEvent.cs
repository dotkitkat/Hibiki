using System.Threading.Tasks;

namespace Hibiki.Events
{
    public static class ReadyEvent
    {
        public static async Task Ready()
        {
            await Logger.LogAsync("Ready.");
        }
    }
}