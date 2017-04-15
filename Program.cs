using System;
using System.Threading.Tasks;

namespace Hibiki
{
    internal class Program
    {
        private static void Main() => StartupAsync().GetAwaiter().GetResult();

        private static async Task StartupAsync()
        {
            await Logger.LogAsync("Startup initialized.", "Bootloader");

            // Run bot

            while (true)
            {
                await Logger.LogAsync("Starting Hibiki...", "Bootloader");
                await new HibikiBot().RunAndBlockAsync();
            }
        }
    }
}