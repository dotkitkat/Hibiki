using System.Threading.Tasks;
using Discord;

namespace Hibiki.Common.Extensions
{
    internal static class SocketTextChannelExtensions
    {
        internal static async Task SendEmbedAsync(this ITextChannel channel, EmbedBuilder embed, bool useTTS = false, RequestOptions options = null)
        {
            await channel.SendMessageAsync(string.Empty, useTTS, embed, options);
        }
    }
}