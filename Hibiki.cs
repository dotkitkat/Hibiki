using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Discord.Net;

namespace Hibiki
{
    internal class HibikiBot
    {
        public DiscordSocketClient Client;
        public CommandService Commands;

        public async Task InitializeAsync()
        {
            Client = new DiscordSocketClient();
            Commands = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
        }

        public async Task RunAndBlockAsync()
        {
            await InitializeAsync();

            await Configuration.LoadAsync();

            var Map = new DependencyMap();
            Map.Add(Client);
            Map.Add(Commands);

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());

            var Search = await Configuration.TrySearchAsync("ApplicationToken");
            if (!Search.Success)
            {
                await Logger.ErrorAsync("Invalid token.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            try
            {
                await Client.LoginAsync(TokenType.Bot, Search.Result);
            }
            catch (HttpException)
            {
                await Logger.ErrorAsync("Invalid token or cannot connect to the Discord gateway.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}