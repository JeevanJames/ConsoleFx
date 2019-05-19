#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Validators;

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
