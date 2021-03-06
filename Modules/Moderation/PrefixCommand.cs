﻿using System.Threading.Tasks;
using Discord.Commands;
using Hibiki.Common.Extensions;
using Hibiki.Common.Language;
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

        [Command("Prefix"), Summary("Change the prefix for a server."), RequirePermission(AccessLevel.ServerOwner)]
        public async Task Prefix(string newPrefix)
        {
            var AllSettings = _Mongo.GetCollection<Settings>();
            var GuildSettings = await AllSettings.GetByGuildAsync(Context.Guild);

            GuildSettings.Prefix = newPrefix;
            await AllSettings.SaveAsync(GuildSettings);
            await Context.Responder()
                .Success()
                .Message((await LanguageManager.GetStringAsync(_Mongo, "change_prefix", Context.Guild)).Replace("{{prefix}}", newPrefix))
                .SendAsync();
        }

        [Command("Prefix"), Summary("View the prefix for a server.")]
        public async Task Prefix()
        {
            await Context.Responder()
                .Emoji(":information_source:")
                .Message((await LanguageManager.GetStringAsync(_Mongo, "current_prefix", Context.Guild)).Replace("{{prefix}}", await PrefixManager.GetPrefixAsync(_Mongo, Context.Guild)))
                .SendAsync();
        }
    }
}