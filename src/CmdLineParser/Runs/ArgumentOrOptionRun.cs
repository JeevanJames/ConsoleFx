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

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Bases;

namespace ConsoleFx.CmdLineParser.Runs
{
    public abstract class ArgumentOrOptionRun<TArg>
        where TArg : ArgumentOrOption<TArg>
    {
        private readonly Converter<string, object> _converter;

        protected ArgumentOrOptionRun(TArg arg)
        {
            Arg = arg;
            Type = arg.Type ?? typeof(string);
            _converter = GetConverter(arg);
        }

        private Converter<string, object> GetConverter(TArg arg)
        {
            Converter<string, object> converter = arg.TypeConverter;

            // If a custom type converter is not specified and the option's value type is not string,
            // then attempt to find a default type converter for that type, which can convert from string.
            if (converter is null && Type != typeof(string))
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(Type);

                // If a default converter cannot be found, throw an exception.
                if (!typeConverter.CanConvertFrom(typeof(string)))
                {
                    throw new ParserException(-1,
                        $"Unable to find a adequate type converter to convert parameters of the {arg.Name} to type {Type.FullName}.");
                }

                converter = value => typeConverter.ConvertFromString(value);
            }

            return converter;
        }

        protected TArg Arg { get; }

        /// <summary>
        ///     Gets the resolved type of the arg.
        /// </summary>
        internal Type Type { get; }

        /// <summary>
        ///     Gets or sets the resolved value of the arg.
        /// </summary>
        internal object Value { get; set; }

        /// <summary>
        ///     Helper method to resolve the raw string value of the arg using the formatter and type
        ///     converter.
        /// </summary>
        /// <param name="rawValue">The raw string value assigned to the arg.</param>
        /// <returns>The resolved value after applying formatter and type converter.</returns>
        internal object ResolveValue(string rawValue)
        {
            string formattedValue = Arg.Formatter != null ? Arg.Formatter(rawValue) : rawValue;
            return _converter != null ? _converter(formattedValue) : formattedValue;
        }

        /// <summary>
        ///     Helper method to create an <see cref="IList{T}"/> object from the arg's type details
        ///     using reflection.
        /// </summary>
        /// <param name="capacity">The initial capacity of the list.</param>
        /// <returns>The created <see cref="IList{T}"/> instance.</returns>
        internal IList CreateCollection(int capacity)
        {
            Type listType = typeof(List<>).MakeGenericType(Type);
            var list = (IList)Activator.CreateInstance(listType, capacity);
            return list;
        }
    }
}
