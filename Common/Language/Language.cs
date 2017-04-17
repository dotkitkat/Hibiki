using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public string FileLocation;
        public string DisplayName;
        public string InternalName;

        internal Language(string displayName, string internalName, string fileLocation)
        {
            FileLocation = fileLocation;
            InternalName = internalName;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    internal static class LanguageManager
    {
        private static readonly List<Language> Supported = new List<Language>
        {
            new Language("English Standard", "en", "Data/english_standard.json"),
            new Language("English Casual", "en-casual", "Data/english_casual.json")
        };

        internal static string GetAllSupported()
        {
            return string.Join(", ", Supported.Select(lang => lang.InternalName));
        }

        internal static List<Language> GetSupported()
        {
            return Supported;
        }

        internal static Language GetLanguage(string internalName = "en")
        {
            return Supported.Find(x => x.InternalName == internalName);
        }

        internal static async Task<string> InternalGetStringAsync(Language language, string desired)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var Results = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(language.FileLocation));
                    return Results[desired];
                });
            }
            catch (Exception e)
            {
                return "An error occurred getting translation " + desired + " for language " + language.DisplayName +
                       ". Please report this to the bot owner!";
            }
        }

        internal static async Task<string> GetStringAsync(MongoClient client, string desired, IGuild guild)
        {
            return await InternalGetStringAsync((await client.GetCollection<Settings>().GetByGuildAsync(guild)).Language, desired);
        }

        internal static async Task ChangeLanguageAsync(MongoClient client, Language desired, IGuild guild)
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