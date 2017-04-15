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
                    var ConfigText = File.ReadAllText(options == null ? @"hibikiconfig.json" : options.ConfigLocation);
                    Core = JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigText);
                }
                catch (Exception e)
                {
                    await Logger.ErrorAsync("Cannot read from configuration file. Exiting...");
                    await Logger.ErrorAsync(e.Message);
                    Console.Read();
                    Environment.Exit(0);
                }
            });
        }

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