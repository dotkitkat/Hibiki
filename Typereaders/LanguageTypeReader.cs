using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Hibiki.Common.Language;

namespace Hibiki.Typereaders
{
    public class LanguageTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            var Map = new Dictionary<string, Languages>
            {
                {"en-casual", Languages.EnglishCasual},
                {"en", Languages.EnglishDefault}
            };

            if (Map.ContainsKey(input))
            {
                return await Task.FromResult(TypeReaderResult.FromSuccess(Map[input]));
            }
            return await Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Invalid language. Available languages are: " + string.Join(", ", Map.Keys)));
        }
    }
}