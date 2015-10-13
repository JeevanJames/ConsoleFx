#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

namespace ConsoleFx.Parser.Styles
{
    public abstract class ParserStyle
    {
        public virtual CommandGrouping GetGrouping(CommandGrouping specifiedGrouping, Options options, Arguments arguments)
        {
            return specifiedGrouping;
        }

        /// <summary>
        /// Validate that the defined options are compatible with the parser style.
        /// An exception should be thrown if any option is invalid.
        /// An example of an invalid option is a short name longer than one character for the UNIX style parser.
        /// </summary>
        /// <param name="options">List of all the defined options.</param>
        public virtual void ValidateDefinedOptions(Options options)
        {
        }

        /// <summary>
        /// Identifies all provided arguments as arguments, options and option parameters.
        /// No rules are checked during thie phase.
        /// </summary>
        /// <param name="args">The provided arguments to identify.</param>
        /// <param name="options">All defined options.</param>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        public abstract IEnumerable<string> IdentifyTokens(IEnumerable<string> args, Options options, Behaviors behaviors);
    }
}
