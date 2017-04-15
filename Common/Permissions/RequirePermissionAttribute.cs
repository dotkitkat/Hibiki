using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Hibiki.Common.Permissions
{
    public enum AccessLevel
    {
        Blocked,
        IsPrivate,
        User,
        ServerModerator,
        ServerOwner,
        BotOwner
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute: PreconditionAttribute
    {
        private static Dictionary<AccessLevel, string> AlTranslations = new Dictionary<AccessLevel, string>
        {
            {AccessLevel.Blocked, "Blocked"},
            {AccessLevel.IsPrivate, "DM Channel"},
            {AccessLevel.User, "User"},
            {AccessLevel.ServerModerator, "Moderator"},
            {AccessLevel.ServerOwner, "Server Owner"},
            {AccessLevel.BotOwner, "Bot Owner"}
        };

        private readonly AccessLevel _Level;
        private MongoClient _Mongo;

        public RequirePermissionAttribute(AccessLevel level)
        {
            _Level = level;
        }

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command,
            IDependencyMap map)
        {
            _Mongo = map.Get<MongoClient>();

            var Access = await GetPermissionsAsync(_Mongo, context, _Level);

            return await Task.FromResult(Access >= _Level ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("This command requires the " + AlTranslations[_Level] + " permission."));
        }

        public async Task<AccessLevel> GetPermissionsAsync(MongoClient mongo, ICommandContext context,
            AccessLevel request)
        {
            if (context.User.IsBot) return AccessLevel.Blocked;

            var Application = await context.Client.GetApplicationInfoAsync();

            if (Application.Owner.Id == context.User.Id) return AccessLevel.BotOwner;

            var User = context.User as SocketGuildUser;
            if (User == null) return AccessLevel.IsPrivate;

            if (context.Guild.OwnerId == User.Id) return AccessLevel.ServerOwner;

            if (User.GuildPermissions.BanMembers || User.GuildPermissions.KickMembers) return AccessLevel.ServerModerator;

            return AccessLevel.User;
        }
    }
}