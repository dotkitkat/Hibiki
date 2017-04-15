using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hibiki;
using Discord.WebSocket;
using Discord.Commands;
using Hibiki.Common.Extensions;
using Hibiki.Database;
using Hibiki.Database.Structures;
using Hibiki.Structures;
using Hibiki.Typereaders;
using MongoDB.Driver;

namespace Hibiki
{
    internal class MessageHandler
    {
        public DiscordSocketClient Client;
        public CommandService Commands;
        public DependencyMap Map;
        public MongoClient Mongo;

        public async Task SetupAsync(DependencyMap map)
        {
            Map = map;
            Client = Map.Get<DiscordSocketClient>();
            Commands = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
            Mongo = Map.Get<MongoClient>();

            Commands.AddTypeReader<ShopItem>(new ShopItemTypeReader());

            Client.MessageReceived += HandleAsync;

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task<bool> HandleAsync(SocketMessage message)
        {
            var ArgPos = 0;
            var Message = message as SocketUserMessage;
            var Author = message.Author as SocketGuildUser;

            var Context = new CommandContext(Client, Message);

            var Prefix = await PrefixManager.GetPrefixAsync(Mongo, Context.Guild);

            if (Message == null || Author == null || Message.Content == Prefix ||
                !(Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos) ||
                  Message.HasStringPrefix(Prefix, ref ArgPos))) return false;

            var SearchResult = Commands.Search(Context, ArgPos);

            try
            {
                var Result = await Commands.ExecuteAsync(Context, ArgPos, Map);
                await Logger.LogAsync(
                    $"Command {(SearchResult.IsSuccess ? SearchResult.Commands.First().Command.Name + " " : "")}executed by {Context.User} in channel #{Context.Channel.Name} on server {Context.Guild.Name}");
                if (Result.IsSuccess) return true;
                string Response = null;

                switch (Result)
                {
                    case SearchResult SResult:
                        return false;

                    case ParseResult PResult:
                        Response = $"There was an error parsing your command: `{PResult.ErrorReason}`";
                        break;

                    case PreconditionResult PcResult:
                        Response = $"A precondition failed: `{PcResult.ErrorReason}`";
                        break;

                    case ExecuteResult EResult:
                        Response =
                            $"Command failed to execute. If this continues, please contact the bot developer.\n **Exception details:** `{EResult.Exception.Message}";
                        break;
                }

                await Context.Responder().Failure().Message(Response).Send();
                return false;
            }

            catch (Exception E)
            {
                await Message.Channel.SendMessageAsync(
                    $":anger: Something went wrong. If this continues, please contact the bot developer.\n **Exception details:** `{E.Message}`");
                return false;
            }
        }
    }
}