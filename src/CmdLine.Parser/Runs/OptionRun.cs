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

namespace ConsoleFx.CmdLine.Parser.Runs
{
    [DebuggerDisplay("Option: {Option.Name} - {Value} (Assigned: {Assigned})")]
    public sealed class OptionRun : ArgumentOrOptionRun<Option>
    {
        internal OptionRun(Option option)
            : base(option)
        {
            ValueType = option.GetOptionValueType();

            // Optimize the size of the Parameters list based on the value type.
            switch (ValueType)
            {
                case OptionValueType.Flag:
                case OptionValueType.Count:
                    Parameters = new List<string>(0);
                    break;
                case OptionValueType.Object:
                    Parameters = new List<string>(1);
                    break;
                default:
                    Parameters = new List<string>();
                    break;
            }
        }

        internal Option Option => Arg;

        /// <summary>
        ///     Gets or sets the number of occurences of the option.
        /// </summary>
        internal int Occurrences { get; set; }

        internal List<string> Parameters { get; }

        internal OptionValueType ValueType { get; }
    }
}
