using Discord.Commands;
using System.Threading.Tasks;
using Hibiki.Common.Permissions;
using Hibiki.Database;
using Hibiki.Database.Structures;
using MongoDB.Driver;

namespace Hibiki.Modules.Moderation
{
    [Name("Moderation")]
    public class SettingsCommand: ModuleBase
    {
        private readonly MongoClient _Mongo;

        public SettingsCommand(IDependencyMap map)
        {
            _Mongo = map.Get<MongoClient>();
        }

        [Command("config"), Summary("Check the current configuration for this server."), RequirePermission(AccessLevel.ServerOwner)]
        public async Task InvokeDefault()
        {
            var Settings = await _Mongo.GetCollection<Settings>().GetByGuildAsync(Context.Guild);

            await Context.Channel.SendMessageAsync(string.Empty, embed: Settings.AsEmbed(Context.Guild));
        }
    }
}