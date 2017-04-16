using System.Collections.Generic;
using System.Text;
using Discord;
using Hibiki.Database.Interfaces;
using MongoDB.Bson;
using Hibiki.Common.Embeds;
using Hibiki.Common.Language;

namespace Hibiki.Database.Structures
{
    public class Settings: IGuildIndexed
    {
        public ObjectId _id { get; set; }

        public ulong GuildId { get; set; }

        public string Prefix { get; set; } = ">";

        public List<string> BadWords { get; set; } = new List<string>(0);

        public Languages Language { get; set; } = Languages.EnglishDefault;

        public EmbedBuilder AsEmbed(IGuild guild)
        {
            var Embed = Common.Embeds.Embed.Success();
            Embed.Title = "Settings for guild " + guild.Name;
            var Description = new StringBuilder();
            Description.AppendLine($"**Guild ID:** {GuildId}");
            Description.AppendLine($"**Prefix:** {Prefix}");
            return Embed.WithDescription(Description.ToString());
        }
    }
}