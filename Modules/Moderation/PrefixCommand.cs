using System.Threading.Tasks;
using Discord.Commands;
using Hibiki.Common.Permissions;
using Hibiki.Database;
using Hibiki.Database.Structures;
using MongoDB.Driver;

namespace Hibiki.Modules.Moderation
{
    [Name("Moderation")]
    public class PrefixCommand: ModuleBase
    {
        private readonly MongoClient _Mongo;

        public PrefixCommand(IDependencyMap map)
        {
            _Mongo = map.Get<MongoClient>();
        }

        [Command("Prefix"), Summary("Changes the prefix for a server."), RequirePermission(AccessLevel.ServerOwner)]
        public async Task ChangePrefix(string newPrefix)
        {
            var AllSettings = _Mongo.GetCollection<Settings>();
            var GuildSettings = await AllSettings.GetByGuildAsync(Context.Guild);

            GuildSettings.Prefix = newPrefix;
            await AllSettings.SaveAsync(GuildSettings);
            await ReplyAsync($"Prefix changed to `{newPrefix}`!");
        }
    }
}