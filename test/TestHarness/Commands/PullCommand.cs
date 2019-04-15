using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;

namespace TestHarness.Commands
{
    public sealed class PushCommand : MultiRepoCommand
    {
        /// <inheritdoc />
        public PushCommand()
            : base("push", "commit")
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Option> GetOptions()
        {
            foreach (Option option in base.GetOptions())
                yield return option;

            yield return new Option("message", "m")
                .UsedAsSingleParameter(optional: false)
                .ValidateAsString(minLength: 1);
        }
    }
}
