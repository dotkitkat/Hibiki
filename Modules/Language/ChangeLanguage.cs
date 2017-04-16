using System.Threading.Tasks;
using Discord.Commands;
using Hibiki.Common.Extensions;
using Hibiki.Common.Language;
using Hibiki.Common.Permissions;
using MongoDB.Driver;

namespace Hibiki.Modules.Language
{
    [Name("Language")]
    public class ChangeLanguage: ModuleBase
    {
        private readonly MongoClient _Mongo;

        public ChangeLanguage(IDependencyMap map)
        {
            _Mongo = map.Get<MongoClient>();
        }

        [Command("Lang"), Summary("Change the language for Hibiki for this guild."),
         RequirePermission(AccessLevel.ServerOwner)]
        public async Task Change(Languages language)
        {
            await LanguageManager.ChangeLanguageAsync(_Mongo, language, Context.Guild);
            await Context.Responder()
                .Message(await LanguageManager.GetStringAsync(_Mongo, "language_test", Context.Guild))
                .SendAsync();
        }
    }
}