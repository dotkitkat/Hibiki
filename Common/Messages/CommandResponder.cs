using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hibiki.Common.Messages
{
    internal class CommandResponder
    {
        private readonly ICommandContext _Context;
        private EmbedBuilder _MessageEmbed;
        private string _Emoji = ":white_check_mark:";
        private string _MessageContent;

        internal CommandResponder(ICommandContext context)
        {
            _Context = context;
        }

        internal CommandResponder Embed(EmbedBuilder embed)
        {
            _MessageEmbed = embed;
            return this;
        }

        internal CommandResponder Message(string message)
        {
            _MessageContent = message;
            return this;
        }

        internal CommandResponder Success()
        {
            _Emoji = ":white_check_mark:";
            return this;
        }

        internal CommandResponder Emoji(string emoji)
        {
            _Emoji = emoji;
            return this;
        }

        internal CommandResponder Failure()
        {
            _Emoji = ":negative_squared_cross_mark:";
            return this;
        }

        internal async Task ReplyAsync(bool useTTS = false, RequestOptions options = null)
        {
            _MessageContent = "**" + _Context.User.Username + "**" + ", " + _MessageContent;
            await SendAsync(useTTS, options);
        }

        internal async Task SendAsync(bool useTTS = false, RequestOptions options = null)
        {
            await _Context.Channel.SendMessageAsync(_Emoji + " | " + (_MessageContent ?? ""), useTTS, _MessageEmbed);
        }

        public override string ToString() => _Emoji + " | " + (_MessageContent ?? "");
    }
}