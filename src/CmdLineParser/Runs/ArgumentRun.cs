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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Base;

namespace ConsoleFx.CmdLineParser.Runs
{
    //internal abstract class ArgumentOrOptionRun<TArg>
    //    where TArg : ArgumentOrOption<TArg>
    //{
    //}
    [DebuggerDisplay("{" + nameof(Argument) + "}")]
    internal sealed class ArgumentRun
    {
        private readonly Converter<string, object> _converter;

        internal ArgumentRun(Argument argument)
        {
            Argument = argument;
            Type = argument.Type ?? typeof(string);
            _converter = GetConverter(argument);
        }

        private Converter<string, object> GetConverter(Argument argument)
        {
            Converter<string, object> converter = argument.TypeConverter;

            // If a custom type converter is not specified and the option's value type is not string,
            // then attempt to find a default type converter for that type, which can convert from string.
            if (converter is null && Type != typeof(string))
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(Type);

                // If a default converter cannot be found, throw an exception.
                if (!typeConverter.CanConvertFrom(typeof(string)))
                {
                    throw new ParserException(-1,
                        $"Unable to find a adequate type converter to convert parameters of the {argument.Name} to type {Type.FullName}.");
                }

                converter = value => typeConverter.ConvertFromString(value);
            }

            return converter;
        }

        internal Argument Argument { get; }

        internal Type Type { get; }

        internal bool Assigned { get; set; }

        internal object Value { get; set; }

        internal object Convert(string value)
        {
            string formattedValue = Argument.Formatter != null ? Argument.Formatter(value) : value;
            return _converter != null ? _converter(formattedValue) : formattedValue;
        }

        internal IList CreateCollection(int capacity)
        {
            Type listType = typeof(List<>).MakeGenericType(Type);
            var list = (IList)Activator.CreateInstance(listType, capacity);
            return list;
        }
    }
}
