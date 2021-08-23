// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLine.Parser.Runs
{
    /// <summary>
    ///     Represents the state of an option while it is being processed. This includes the number
    ///     of occurrences, the specified parameters and the expected type for the property that holds
    ///     the option's value.
    /// </summary>
    [DebuggerDisplay("Option: {Option.Name} - {Value} (Assigned: {Assigned})")]
    public sealed class OptionRun : ArgumentOrOptionRun<Option>
    {
        internal OptionRun(Option option)
            : base(option)
        {
            ValueType = option.GetValueType();

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
