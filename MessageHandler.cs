using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Hibiki;
using Discord.WebSocket;
using Discord.Commands;
using Hibiki.Common.Extensions;
using Hibiki.Common.Language;
using Hibiki.Database;
using Hibiki.Database.Structures;
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

            Client.MessageReceived += HandleAsync;

            SetupCommmandsErrorEvent();

            Commands.AddTypeReader<Languages>(new LanguageTypeReader());

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public void SetupCommmandsErrorEvent()
        {
            Commands.Log += async arg =>
            {
                await Logger.LogAsync(arg.Exception.InnerException.Message);
            };
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
                    $"Command {(SearchResult.IsSuccess ? SearchResult.Commands.First().Command.Name + " was successfully executed" : "failed to execute")} by {Context.User} in channel #{Context.Channel.Name} on server {Context.Guild.Name}");
                if (Result.IsSuccess) return true;
                string Response = null;

                switch (Result)
                {
                    case SearchResult SResult:
                        return false;

                    case ParseResult PResult:
                        Response = PResult.Error == CommandError.BadArgCount
                            ? await LanguageManager.GetStringAsync(Mongo, "errors_insufficient_arguments",
                                Context.Guild)
                            : (await LanguageManager.GetStringAsync(Mongo, "errors_parse_failed", Context.Guild))
                            .Replace("{{parseFailedReason}}", PResult.ErrorReason);
                        break;

                    case PreconditionResult PcResult:
                        Response =
                            (await LanguageManager.GetStringAsync(Mongo, "errors_precondition_failed", Context.Guild))
                            .Replace("{{preconditionFailedReason}}", PcResult.ErrorReason);
                        break;

                    case ExecuteResult EResult:
                        Response =
                            (await LanguageManager.GetStringAsync(Mongo, "errors_execution_failed", Context.Guild)).Replace("{{executeFailedReason}}", EResult.Exception.Message);
                        break;
                }

                await Context.Responder().Failure().Message(Response).SendAsync();
                return false;
            }

            catch (Exception E)
            {
                await Message.Channel.SendMessageAsync(
                    (await LanguageManager.GetStringAsync(Mongo, "errors_commands_unknown", Context.Guild)).Replace("{{commandFailedReason}}", E.Message));
                return false;
            }
        }
    }
}