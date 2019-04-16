using System;
using System.Collections.Generic;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;

namespace TestHarness
{
    public class MultiRepoCommand : HelpCommand
    {
        /// <inheritdoc />
        public MultiRepoCommand(params string[] names)
            : base(names)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Option> GetOptions()
        {
            IEnumerable<Option> options = base.GetOptions();
            foreach (Option option in options)
                yield return option;

            yield return new Option("include", "i")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                .ValidateWithRegex(@"^[\w_-]+$");

            yield return new Option("exclude", "e")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                .ValidateWithRegex(@"^[\w_-]+$");
        }

        /// <inheritdoc />
        protected override string PerformCustomValidation(IReadOnlyList<string> arguments, IReadOnlyDictionary<string, object> options)
        {
            if (options["include"] != null && options["exclude"] != null)
                return "Cannot specify both include and exclude";
            return base.PerformCustomValidation(arguments, options);
        }
    }
}
