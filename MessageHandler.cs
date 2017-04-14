using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hibiki;
using Discord.WebSocket;
using Discord.Commands;

namespace Hibiki
{
    internal class MessageHandler
    {
        public DiscordSocketClient Client;
        public CommandService Commands;
        public DependencyMap Map;

        public async Task SetupAsync(DependencyMap map)
        {
            Map = map;
            Client = Map.Get<DiscordSocketClient>();
            Commands = new CommandService(new CommandServiceConfig()
            {
                DefaultRunMode = RunMode.Async
            });

            Client.MessageReceived += HandleAsync;

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task<bool> HandleAsync(SocketMessage message)
        {
            var argPos = 0;
            var Message = message as SocketUserMessage;
            var Author = message.Author as SocketGuildUser;

            var Context = new CommandContext(Client, Message);

            var ConfigResult = await Configuration.TrySearchAsync("Prefix");
            if (!ConfigResult.Success) throw new ConfigurationException();

            if (Message == null || Author == null || Message.Content == ConfigResult.Result ||
                !(Message.HasMentionPrefix(Client.CurrentUser, ref argPos) ||
                  Message.HasStringPrefix(ConfigResult.Result, ref argPos))) return false;

            var SearchResult = Commands.Search(Context, argPos);

            try
            {
                var Result = await Commands.ExecuteAsync(Context, argPos, Map);
                await Logger.LogAsync(
                    $"Command {(SearchResult.IsSuccess ? SearchResult.Commands.First().Command.Name + " " : "")}executed by {Context.User} in channel #{Context.Channel.Name} on server {Context.Guild.Name}");
                if (Result.IsSuccess) return true;
                string Response = null;

                switch (Result)
                {
                    case SearchResult SResult:
                        return false;

                    case ParseResult PResult:
                        Response = $":anger: There was an error parsing your command: `{PResult.ErrorReason}";
                        break;

                    case PreconditionResult PcResult:
                        Response = $":anger: A precondition failed: `{PcResult.ErrorReason}`";
                        break;

                    case ExecuteResult EResult:
                        Response =
                            $":anger: Command failed to execute. If this continues, please contact the bot developer.\n **Exception details:** `{EResult.Exception.Message}";
                        break;
                }

                await Message.Channel.SendMessageAsync(Response);
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