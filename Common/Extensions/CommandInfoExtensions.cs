using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;

namespace Hibiki.Common.Extensions
{
    public static class CommandInfoExtensions
    {
        public static async Task<IEnumerable<CommandInfo>> CheckConditionsAsync(this IEnumerable<CommandInfo> commandInfos,
            ICommandContext context, IDependencyMap map = null)
        {
            var Commands = new List<CommandInfo>();
            foreach (var CommandInfo in commandInfos)
            {
                if ((await CommandInfo.CheckPreconditionsAsync(context, map)).IsSuccess)
                {
                    Commands.Add(CommandInfo);
                }
            }
            return Commands;
        }
    }
}