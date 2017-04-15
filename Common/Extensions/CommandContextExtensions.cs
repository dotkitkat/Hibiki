using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Hibiki.Common.Messages;

namespace Hibiki.Common.Extensions
{
    internal static class CommandContextExtensions
    {
        internal static async Task EmbedAsync(this ICommandContext context, EmbedBuilder embed, bool useTTS = false, RequestOptions options = null)
        {
            await context.Channel.SendMessageAsync(string.Empty, useTTS, embed, options);
        }

        internal static CommandResponder Responder(this ICommandContext context)
        {
            return new CommandResponder(context);
        }
    }
}