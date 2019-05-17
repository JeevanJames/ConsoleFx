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

using ConsoleFx.CmdLineArgs;

namespace ConsoleFx.CmdLineParser.Runs
{
    [DebuggerDisplay("{Option.Name} - {Value}")]
    public sealed class OptionRun
    {
        private readonly Converter<string, object> _converter;

        internal OptionRun(Option option)
        {
            Option = option;
            Type = option.Type ?? typeof(string);
            _converter = GetConverter(option);
            ValueType = GetOptionValueType(option);
        }

        private Converter<string, object> GetConverter(Option option)
        {
            Converter<string, object> converter = option.TypeConverter;

            // If a custom type converter is not specified and the option's value type is not string,
            // then attempt to find a default type converter for that type, which can convert from string.
            if (converter is null && Type != typeof(string))
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(Type);

                // If a default converter cannot be found, throw an exception.
                if (!typeConverter.CanConvertFrom(typeof(string)))
                {
                    throw new ParserException(-1,
                        $"Unable to find a adequate type converter to convert parameters of the {option.Name} to type {Type.FullName}.");
                }

                converter = value => typeConverter.ConvertFromString(value);
            }

            return converter;
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

        internal Option Option { get; }

        internal Type Type { get; }

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
        internal object Value { get; set; }

        internal OptionValueType ValueType { get; set; }

        internal object Convert(string value)
        {
            string formattedValue = Option.Formatter != null ? Option.Formatter(value) : value;
            return _converter != null ? _converter(formattedValue) : formattedValue;
        }
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
