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
            Logger.DebugAsync("Accessing Hibiki Database").GetAwaiter().GetResult();
            var Db = mongo.GetDatabase("Hibiki");
            return Db.GetCollection<T>(typeof(T).Name);
        }

        internal static async Task<T> GetByGuildAsync<T>(this IMongoCollection<T> collection, IGuild guild)
            where T : IGuildIndexed, new()
        {
            await Logger.DebugAsync("GBGA_ACCESS_DELIMITED");
            IAsyncCursor<T> Cursor;
            try
            {
                Cursor = await collection.FindAsync(g => g.GuildId == guild.Id);
            }
            catch (Exception e)
            {
                await Logger.DebugAsync(e.StackTrace + e.Message + e.Source);
                Cursor = null;
            }
            await Logger.DebugAsync("FOUND_GUILD");
            var Result = await Cursor.FirstOrDefaultAsync();

            await Logger.DebugAsync("GBGA_PROC_DB_1");
            await Logger.DebugAsync(Result != null ? "RESULT_NOT_NULL" : "NO_RESULT");
            if (Result != null) return Result;
            await Logger.DebugAsync("RESULT_TR_NULL");

            await collection.CreateGuildEntryAsync(guild);
            var CursorNew = await collection.FindAsync(g => g.GuildId == guild.Id);
            return await CursorNew.FirstOrDefaultAsync();
        }

        internal static async Task<T> GetByUserAsync<T>(this IMongoCollection<T> collection, IUser user)
            where T : IUserIndexed, new()
        {
            var Cursor = await collection.FindAsync(g => g.UserId == user.Id);
            var Result = await Cursor.FirstOrDefaultAsync();

            if (Result != null) return Result;

            await collection.CreateUserEntryAsync(user);
            var NewCursor = await collection.FindAsync(g => g.UserId == user.Id);
            return await NewCursor.FirstOrDefaultAsync();
        }

        internal static async Task<ReplaceOneResult> CreateUserEntryAsync<T>(this IMongoCollection<T> collection,
            IUser user) where T : IUserIndexed, new()
        {
            return await collection.SaveUserAsync(new T
            {
                UserId = user.Id
            });
        }

        internal static async Task<ReplaceOneResult> CreateGuildEntryAsync<T>(this IMongoCollection<T> collection, IGuild guild) where T: IGuildIndexed, new()
        {
            return await collection.SaveAsync(new T
            {
                GuildId = guild.Id
            });
        }

        internal static async Task<ReplaceOneResult> SaveAsync<T>(this IMongoCollection<T> collection, T entity) where T : IGuildIndexed
        {
            return await collection.ReplaceOneAsync(i => i._id == entity._id, entity, new UpdateOptions { IsUpsert = true });
        }

        internal static async Task<ReplaceOneResult> SaveUserAsync<T>(this IMongoCollection<T> collection, T entity)
            where T : IUserIndexed
        {
            return await collection.ReplaceOneAsync(i => i._id == entity._id, entity,
                new UpdateOptions {IsUpsert = true});
        }
    }
}