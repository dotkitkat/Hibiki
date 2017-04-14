using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hibiki
{
    internal static class Configuration
    {
        private static Dictionary<string, string> Core = new Dictionary<string, string>();

        public static async Task LoadAsync(LoadOptions options = null)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var ConfigText = File.ReadAllText(@"hibikiconfig.json");
                    Core = JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigText);
                }
                catch (Exception)
                {
                    Console.ReadKey();
                    await Logger.ErrorAsync("Cannot read from configuration file. Exiting...");
                    Environment.Exit(0);
                }
            });
        }

        /// <summary>
        /// Searchs the current configuration for the specified key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>Result container. See <see cref="ConfigurationSearchResult" /></returns>
        public static async Task<ConfigurationSearchResult> TrySearchAsync(string key)
        {
            return await Task.Run(() =>
            {
                string Result;
                var CSearch = new ConfigurationSearchResult();
                var Success = Core.TryGetValue(key, out Result);
                CSearch.Success = Success;
                CSearch.Result = Result;
                return CSearch;
            });
        }
    }

    public class ConfigurationSearchResult
    {
        public bool Success;
        public string Result;
    }

    public class LoadOptions
    {
        public string ConfigLocation = @"hibikiconfig.json";
    }
}