// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Validators;

namespace TestHarness
{
    public class MultiRepoCommand : Command
    {
        public MultiRepoCommand(params string[] names)
            : base(names)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Arg> GetArgs()
        {
            IEnumerable<Arg> args = base.GetArgs();
            foreach (Arg arg in args)
                yield return arg;

            yield return new Option("include", "i")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                .ValidateWithRegex(@"^[\w_-]+$");

            yield return new Option("exclude", "e")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                .ValidateWithRegex(@"^[\w_-]+$");
        }

        /// <inheritdoc />
        protected override string PerformCustomValidation(IReadOnlyList<object> arguments, IReadOnlyDictionary<string, object> options)
        {
            if (options["include"] != null && options["exclude"] != null)
                return "Cannot specify both include and exclude";
            return base.PerformCustomValidation(arguments, options);
        }
    }
}
