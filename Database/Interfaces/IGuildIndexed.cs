using MongoDB.Bson;

namespace Hibiki.Database.Interfaces
{
    public interface IGuildIndexed
    {
        // ReSharper disable once InconsistentNaming
        ObjectId _id { get; set; }
        ulong GuildId { get; set; }
    }
}