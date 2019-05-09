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
using System.Linq;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Program
{
    public class HelpCommand : Command
    {
        public HelpCommand(params string[] names)
            : base(names)
        {
        }

        public HelpCommand(bool caseSensitive, params string[] names)
            : base(caseSensitive, names)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Option> GetOptions()
        {
            IEnumerable<Option> options = base.GetOptions();
            foreach (Option option in options)
                yield return option;

            yield return new Option("help", "h");
        }

        /// <inheritdoc />
        protected override string PerformCustomValidation(IReadOnlyList<object> arguments, IReadOnlyDictionary<string, object> options)
        {
            if (options["help"] != null)
            {
                if (arguments.Count > 0)
                    return "Cannot specify any other arguments with help";
                if (options.Any(opt => opt.Value != null && opt.Key != "help"))
                    return "Cannot specify any other arguments with help";
            }

            return base.PerformCustomValidation(arguments, options);
        }

        /// <inheritdoc />
        protected override int HandleCommand(IReadOnlyList<object> arguments, IReadOnlyDictionary<string, object> options)
        {
            if (options["help"] != null)
                ConsoleEx.PrintLine(new ColorString().BgBlue("This is the help text"));
            return base.HandleCommand(arguments, options);
        }
    }
}
