using Discord.Commands;
using System.Threading.Tasks;
using Hibiki.Database;
using Hibiki.Database.Structures;
using MongoDB.Driver;

namespace Hibiki.Modules.Development
{
    [Name("Development")]
    public class DatabaseTestingCommand: ModuleBase
    {
        private readonly MongoClient _Mongo;

        public DatabaseTestingCommand(IDependencyMap map)
        {
            _Mongo = map.Get<MongoClient>();
        }

        [Command("DBTest"), Summary("Used for testing the Mongo database."), RequireOwner]
        public async Task InvokeDefault()
        {
            var Settings = await _Mongo.GetCollection<Settings>().GetByGuildAsync(Context.Guild);

            await Context.Channel.SendMessageAsync(string.Empty, embed: Settings.AsEmbed(Context.Guild));
        }
    }
}