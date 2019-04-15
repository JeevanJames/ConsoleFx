using System.Collections.Generic;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;

namespace TestHarness
{
    public class MultiRepoCommand : Command
    {
        /// <inheritdoc />
        public MultiRepoCommand(params string[] names)
            : base(names)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Option> GetOptions()
        {
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
            return base.PerformCustomValidation(arguments, options);
        }
    }
}
