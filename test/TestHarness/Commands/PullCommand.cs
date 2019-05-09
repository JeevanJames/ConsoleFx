using System;
using System.Collections.Generic;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Validators;

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
            foreach (var option in base.GetOptions())
                yield return option;

            yield return new Option("message", "m")
                .UsedAsSingleParameter(optional: false)
                .ValidateAsString(minLength: 1);
        }

        /// <inheritdoc />
        protected override int HandleCommand(IReadOnlyList<object> arguments, IReadOnlyDictionary<string, object> options)
        {
            Console.WriteLine("Pushing");
            return 0;
        }
    }
}
