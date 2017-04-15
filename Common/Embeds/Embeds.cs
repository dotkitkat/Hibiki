using Discord;

namespace Hibiki.Common.Embeds
{
    internal static class Embed
    {
        internal static EmbedBuilder Success()
        {
            return new EmbedBuilder {Color = new Color(51, 204, 51)};
        }

        internal static EmbedBuilder Error()
        {
            return new EmbedBuilder {Color = new Color(255, 0, 0)};
        }

        internal static EmbedBuilder Info()
        {
            return new EmbedBuilder {Color = new Color(51, 102, 255)};
        }
    }
}