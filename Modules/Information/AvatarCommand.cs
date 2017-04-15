using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hibiki.Modules.Information
{
    [Name("Information")]
    public class AvatarCommand: ModuleBase
    {
        [Command("Avatar"), Summary("Gets the avatar for a user.")]
        public async Task Avatar(IGuildUser user = null)
        {
            var Embed = Common.Embeds.Embed.Success();
            var User = user ?? Context.User;
            Embed.ImageUrl = User.GetAvatarUrl();
            Embed.Author = new EmbedAuthorBuilder()
            {
                IconUrl = User.GetAvatarUrl(),
                Name = User.Username
            };
            await Context.Channel.SendMessageAsync(string.Empty, embed: Embed);
        }
    }
}