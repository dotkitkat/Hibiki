using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Hibiki.Database;
using Hibiki.Database.Structures;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Hibiki.Common.Language
{
    public class Language
    {

    }

    public enum Languages
    {
        EnglishDefault,
        EnglishCasual
    }

    internal static class LanguageManager
    {
        internal static Dictionary<Languages, string> LanguageFiles = new Dictionary<Languages, string>
        {
            {Languages.EnglishDefault, "Data/english_default.json"},
            {Languages.EnglishCasual, "Data/english_casual.json"}
        };

        internal static async Task<string> InternalGetStringAsync(Languages language, string desired)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var Results = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(LanguageFiles[language]));
                    return Results[desired];
                });
            }
            catch (Exception e)
            {
                throw new LanguageException("An error occurred getting " + desired + " for language " + language);
            }
        }

        internal static async Task<string> GetStringAsync(MongoClient client, string desired, IGuild guild)
        {
            var Guild = await client.GetCollection<Settings>().GetByGuildAsync(guild);
            return await InternalGetStringAsync(Guild.Language, desired);
        }

        internal static async Task ChangeLanguageAsync(MongoClient client, Languages desired, IGuild guild)
        {
            var Settings = client.GetCollection<Settings>();
            var Guild = await Settings.GetByGuildAsync(guild);
            Guild.Language = desired;
            await Settings.SaveAsync(Guild);
        }
    }

    internal class LanguageException : Exception
    {
        public LanguageException(string message)
        {

        }
    }
}