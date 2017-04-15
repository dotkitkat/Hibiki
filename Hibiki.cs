using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Discord.Net;
using Hibiki.Database;
using MongoDB.Driver;
using Hibiki.Events;

namespace Hibiki
{
    internal class HibikiBot
    {
        public DiscordSocketClient Client;
        public CommandService Commands;
        public MongoClient Mongo;

        public async Task InitializeAsync()
        {
            await Task.Run(async () =>
            {
                await Configuration.LoadAsync();
                Client = new DiscordSocketClient();
                var MongoResult = await Configuration.TrySearchAsync("MongoIp");
                Mongo = MongoResult.Success ? new MongoClient(MongoResult.Result) : new MongoClient("mongodb://localhost:27017");
            });
        }

        public async Task RunAndBlockAsync()
        {
            await InitializeAsync();

            var Map = new DependencyMap();
            Map.Add(Client);
            Map.Add(Mongo);

            var Manager = new EventManager();
            await Manager.RegisterEventsAsync(Map);

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