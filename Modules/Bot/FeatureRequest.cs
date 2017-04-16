using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hibiki.Common.Extensions;
using Hibiki.Common.Permissions;

namespace Hibiki.Modules.Bot
{
    [Name("Bot")]
    public class FeatureRequest: ModuleBase
    {
        [Command("Request"), Summary("Request a feature for Hibiki."), RequirePermission(AccessLevel.ServerOwner)]
        public async Task RequestCommand([Remainder] string requestContent)
        {
            var RequestsChannel = await Context.Client.GetChannelAsync(302650525097263106) as SocketTextChannel;
            if (RequestsChannel == null)
            {
                await ReplyAsync("Error sending request.");
                return;
            }
            var Embed = Common.Embeds.Embed.Info();
            Embed.Title = "Feature Request";
            Embed.Description = requestContent;
            Embed.AddInlineField("Author", Context.User.Username);
            await RequestsChannel.SendEmbedAsync(Embed);
        }
    }
}