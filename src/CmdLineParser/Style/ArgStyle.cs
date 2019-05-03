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

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineParser.Runs;

namespace ConsoleFx.CmdLineParser.Style
{
    /// <summary>
    ///     Base class that defines the style of the arguments being parsed.
    /// </summary>
    public abstract class ArgStyle
    {
        /// <summary>
        ///     Allows the parser style to override the preferred grouping based on its rules for the
        ///     specified options and arguments.
        /// </summary>
        /// <param name="specifiedGrouping">The preferred grouping.</param>
        /// <param name="options">The list of allowed options.</param>
        /// <param name="arguments">The list of allowed arguments.</param>
        /// <returns>The final grouping for the specified options and arguments.</returns>
        public virtual ArgGrouping GetGrouping(ArgGrouping specifiedGrouping, IReadOnlyList<Option> options,
            IReadOnlyList<Argument> arguments)
        {
            return specifiedGrouping;
        }

        /// <summary>
        ///     <para>Validate that the defined options are compatible with the parser style.</para>
        ///     <para>An exception should be thrown if any option is invalid.</para>
        ///     <para>An example of an invalid option is a short name longer than one character for the UNIX style parser.</para>
        /// </summary>
        /// <param name="options">List of all the defined options.</param>
        public virtual void ValidateDefinedOptions(IEnumerable<Option> options)
        {
        }

        /// <summary>
        ///     <para>Identifies all provided tokens as arguments, options and option parameters.</para>
        ///     <para>Option and argument validators are not checked in this phase. Only the arg grouping is checked.</para>
        /// </summary>
        /// <param name="tokens">All the specified tokens.</param>
        /// <param name="options">All available options. If any of the tokens match, add its details to this parameter.</param>
        /// <param name="grouping">The expected arg grouping to validate.</param>
        /// <returns>A collection of all the identified arguments.</returns>
        public abstract IEnumerable<string> IdentifyTokens(IEnumerable<string> tokens, IReadOnlyList<OptionRun> options,
            ArgGrouping grouping);

        public static readonly ArgStyle Unix = new UnixArgStyle();

        public static readonly ArgStyle Windows = new WindowsArgStyle();
    }
}
