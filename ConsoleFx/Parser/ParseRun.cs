#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

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

namespace ConsoleFx.Parser
{
    /// <summary>
    ///     Represents the internal state of a single parse execution. This includes the commands, arguments and options that
    ///     were specified.
    /// </summary>
    internal sealed class ParseRun
    {
        /// <summary>
        ///     All specified commands.
        ///     Note: We use a <see cref="List{T}" /> instead of the <see cref="Commands"/> collection here, because we want to avoid the
        ///     duplicate checks, as commands at different levels can have the same name.
        /// </summary>
        internal List<Command> Commands { get; } = new List<Command>();

        /// <summary>
        ///     All allowed arguments and their values.
        /// </summary>
        internal List<ArgumentRun> Arguments { get; } = new List<ArgumentRun>();

        /// <summary>
        ///     All allowed options and details of which are specified.
        /// </summary>
        internal List<OptionRun> Options { get; } = new List<OptionRun>();

        /// <summary>
        ///     All the specified options and argument tokens after accounting for the commands.
        /// </summary>
        internal List<string> Tokens { get; set; }
    }

    internal sealed class ArgumentRun
    {
        internal ArgumentRun(Argument argument)
        {
            Argument = argument;
        }

        internal Argument Argument { get; }

        internal string Value { get; set; }
    }

    public sealed class OptionRun
    {
        internal OptionRun(Option option, Command command)
        {
            Option = option;
            Command = command;
        }

        internal Option Option { get; }

        internal Command Command { get; }

        internal int Occurences { get; set; }

        //TODO: Optimize initial capacity of this list based on the min and max parameters of the option.
        internal List<string> Parameters { get; } = new List<string>();

        /// <summary>
        ///     <para>The final value of the parameters of the option. The actual type depends on how the option is setup.</para>
        ///     If the option allows parameters, then this can be:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>a <see cref="IList{T}" />, if more than one parameters are allowed, or</description>
        ///         </item>
        ///         <item>
        ///             <description>an object of type T, if only one parameter is allowed.</description>
        ///         </item>
        ///     </list>
        ///     If the option does not allow parameters, then this can be:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 an <see cref="int" /> which is the number of times the option is specified (if it allows more
        ///                 than one occurence), or
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 an <see cref="bool" /> which is true if the option was specified otherwise false (if it allows
        ///                 only one occurence).
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        internal object ResolvedValue { get; set; }

        internal void Clear()
        {
            Occurences = 0;
            Parameters.Clear();
        }
    }
}
