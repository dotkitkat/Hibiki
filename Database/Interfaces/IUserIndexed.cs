using MongoDB.Bson;

namespace Hibiki.Database.Interfaces
{
    public interface IUserIndexed
    {
        // ReSharper disable once InconsistentNaming
        ObjectId _id { get; set; }
        ulong UserId { get; set;  }
    }
}