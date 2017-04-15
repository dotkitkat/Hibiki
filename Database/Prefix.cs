using System.Threading.Tasks;
using Discord;
using Hibiki.Database.Structures;
using MongoDB.Driver;

namespace Hibiki.Database
{
    internal static class PrefixManager
    {
        internal static async Task<string> GetPrefixAsync(MongoClient client, IGuild guild)
        {
            string Prefix;

            var AllSettings = client.GetCollection<Settings>();
            var GuildSettings = await AllSettings.GetByGuildAsync(guild);

            if (GuildSettings.Prefix == null)
            {
                var Result = await Configuration.TrySearchAsync("Prefix");
                if (Result.Success)
                {
                    Prefix = Result.Result;
                }
                else
                {
                    throw new ConfigurationException();
                }
            }
            else
            {
                Prefix = GuildSettings.Prefix;
            }

            return Prefix;
        }
    }
}