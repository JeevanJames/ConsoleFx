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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using ConsoleFx.CmdLine;

namespace ConsoleFx.CmdLine.Parser.Runs
{
    [DebuggerDisplay("{Option.Name} - {Value}")]
    public sealed class OptionRun : ArgumentOrOptionRun<Option>
    {
        internal OptionRun(Option option)
            : base(option)
        {
            ValueType = GetOptionValueType(option);
        }

        private OptionValueType GetOptionValueType(Option option)
        {
            // If parameters are not allowed on the option...
            if (option.Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
            {
                // If the option can occur more than once, it's value will be an integer specifying
                // the number of occurences.
                if (option.Usage.MaxOccurrences > 1)
                    return OptionValueType.Count;

                // If the option can occur not more than once, it's value will be a bool indicating
                // whether it was specified or not.
                return OptionValueType.Flag;
            }

            // If the option can have multiple parameter values (either because the MaxParameters usage
            // is greater than one or because MaxParameters is one but MaxOccurences is greater than
            // one), then the option's value is an IList<Type>.
            if (option.Usage.MaxParameters > 1 || (option.Usage.MaxParameters > 0 && option.Usage.MaxOccurrences > 1))
                return OptionValueType.List;

            // If the option only has one parameter specified, then the option's value is a string.
            if (option.Usage.MaxParameters == 1 && option.Usage.MaxOccurrences == 1)
                return OptionValueType.Object;

            //TODO: Change this to an internal parser exception.
            throw new InvalidOperationException("Should never reach here.");
        }

        internal Option Option => Arg;

        /// <summary>
        ///     Gets or sets the number of occurences of the option.
        /// </summary>
        internal int Occurrences { get; set; }

        //TODO: Optimize initial capacity of this list based on the min and max parameters of the option.
        internal List<string> Parameters { get; } = new List<string>();

        internal OptionValueType ValueType { get; set; }

        //TODO: If this makes sense, move it to the base class.
        internal bool Assigned { get; set; }
    }

    /// <summary>
    ///     The type of the resolved value of an option.
    ///     <para/>
    ///     Decided based on the usage specs of the option.
    /// </summary>
    internal enum OptionValueType
    {
        Object,
        List,
        Count,
        Flag,
    }
}
