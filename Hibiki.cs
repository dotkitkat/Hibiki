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
        }

        public async Task RunAndBlockAsync()
        {
            await InitializeAsync();

            await Configuration.LoadAsync();

            var Map = new DependencyMap();
            Map.Add(Client);

            var MessageHandler = new MessageHandler();
            await MessageHandler.SetupAsync(Map);

            var Search = await Configuration.TrySearchAsync("ApplicationToken");
            if (!Search.Success)
            {
                await Logger.ErrorAsync("Invalid token.");
                Console.Read();
                Environment.Exit(0);
            }

            try
            {
                await Client.LoginAsync(TokenType.Bot, Search.Result);
            }

            catch (HttpException)
            {
                await Logger.ErrorAsync("Invalid token or cannot connect to the Discord gateway.");
                Console.Read();
                Environment.Exit(0);
            }

            try
            {
                await Client.StartAsync();
            }

            catch (Exception)
            {
                await Logger.ErrorAsync("Failed to start.");
                Console.Read();
                Environment.Exit(0);
            }

            await Task.Delay(-1);
        }
    }
}