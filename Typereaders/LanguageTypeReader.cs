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
            var Lang = LanguageManager.GetLanguage(input);
            if (Lang != null)
            {
                return await Task.FromResult(TypeReaderResult.FromSuccess(Lang));
            }
            return await Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Invalid language. Available languages are: " + LanguageManager.GetAllSupported()));
        }
    }
}