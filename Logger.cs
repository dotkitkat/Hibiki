using System;
using System.Threading.Tasks;

namespace Hibiki
{
    internal static class Logger
    {
        internal static async Task LogAsync(string text, string category = null)
        {
            await Console.Out.WriteLineAsync($"[Hibiki] {(category != null ? "[" + category + "] " : "")}{text}");
        }

        internal static async Task ErrorAsync(string text)
        {
            await LogAsync(text, "Error");
        }
    }
}