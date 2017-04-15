using Discord;
using Hibiki.Database.Interfaces;
using MongoDB.Bson;

namespace Hibiki.Database.Structures
{
    public class JoinableRoles: IGuildIndexed
    {
        public ObjectId _id { get; set; }
        public ulong GuildId { get; set; }

        public IRole[] Roles { get; set; }
    }
}