using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Hibiki.Modules.Information
{
    [Name("Information")]
    public class RoleInfoCommand: ModuleBase
    {
        [Command("RoleInfo"), Summary("Gets information for a role."), RequireContext(ContextType.Guild)]
        public async Task InvokeDefault(IRole role)
        {
            var Embed = new EmbedBuilder { Title = "Information for role " + role.Name };
            Embed.AddInlineField("ID", role.Id);
            Embed.AddInlineField("Guild", role.Guild.Name);
            Embed.AddInlineField("Position", role.Position);
            await Context.Channel.SendMessageAsync("", embed: Embed);
        }
    }
}