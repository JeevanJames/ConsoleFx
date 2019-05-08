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
using System.Diagnostics;

using ConsoleFx.CmdLineArgs;

namespace ConsoleFx.CmdLineParser.Runs
{
    [DebuggerDisplay("{Option.ToString()} - {ResolvedValue}")]
    public sealed class OptionRun
    {
        internal OptionRun(Option option)
        {
            Option = option;
        }

        internal Option Option { get; }

        /// <summary>
        ///     Gets or sets the number of occurences of the option.
        /// </summary>
        internal int Occurrences { get; set; }

        //TODO: Optimize initial capacity of this list based on the min and max parameters of the option.
        internal List<string> Parameters { get; } = new List<string>();

        /// <summary>
        ///     Gets or sets the final value of the parameters of the option. The actual type depends
        ///     on how the option is setup.
        ///     <para />
        ///     If the option allows parameters, then this can be:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 a <see cref="IList{T}" />, if more than one parameters are allowed, or
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>an object of type T, if only one parameter is allowed.</description>
        ///         </item>
        ///     </list>
        ///     If the option does not allow parameters, then this can be:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 an <see cref="int" /> which is the number of times the option is specified
        ///                 (if it allows more than one occurence), or
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 a <see cref="bool" /> which is true if the option was specified otherwise
        ///                 false (if it allows only one occurence).
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        internal object ResolvedValue { get; set; }
    }
}
