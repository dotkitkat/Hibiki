using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using Discord;
using Hibiki.Database.Interfaces;
using Hibiki.Database.Structures;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Hibiki.Database
{
    internal static class MongoExtensions
    {
        internal static IMongoCollection<T> GetCollection<T>(this MongoClient mongo)
        {
            var Db = mongo.GetDatabase("Hibiki");
            return Db.GetCollection<T>(typeof(T).Name);
        }

        internal static async Task<T> GetByGuildAsync<T>(this IMongoCollection<T> collection, IGuild guild)
            where T : IGuildIndexed, new()
        {
            var Cursor = await collection.FindAsync(g => g.GuildId == guild.Id);
            var Result = await Cursor.FirstOrDefaultAsync();

            if (Result != null) return Result;

            await collection.CreateGuildEntryAsync(guild);
            var CursorNew = await collection.FindAsync(g => g.GuildId == guild.Id);
            return await Cursor.FirstOrDefaultAsync();
        }

        internal static async Task CreateGuildEntryAsync<T>(this IMongoCollection<T> collection, IGuild guild) where T: IGuildIndexed, new()
        {
            await collection.InsertOneAsync(new T
            {
                GuildId = guild.Id
            });
        }

        internal static async Task<ReplaceOneResult> SaveAsync<T>(this IMongoCollection<T> collection, T entity) where T : IGuildIndexed
        {
            return await collection.ReplaceOneAsync(i => i._id == entity._id, entity, new UpdateOptions { IsUpsert = true });
        }
    }
}