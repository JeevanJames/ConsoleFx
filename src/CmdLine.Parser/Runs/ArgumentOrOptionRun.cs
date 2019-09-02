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
using System.Linq;
using System.Reflection;

namespace ConsoleFx.CmdLine.Parser.Runs
{
    public abstract class ArgumentOrOptionRun<TArg>
        where TArg : ArgumentOrOption<TArg>
    {
        private readonly Converter<string, object> _converter;

        protected ArgumentOrOptionRun(TArg arg)
        {
            Arg = arg;
            Type = arg.Type ?? typeof(string);

            // Pre-determine the type converter needed for this arg.
            _converter = GetConverter(arg);
        }

        private Converter<string, object> GetConverter(TArg arg)
        {
            Converter<string, object> converter = arg.TypeConverter;

            if (converter != null || Type == typeof(string))
                return converter;

            // Look for a type converter that can convert from string.
            TypeConverter typeConverter = TypeDescriptor.GetConverter(Type);
            if (typeConverter.CanConvertFrom(typeof(string)))
                return value => typeConverter.ConvertFromString(value);

            // Look for constructors on the type that can be used to instantiate an instance.
            IEnumerable<ConstructorInfo> ctors = Type.GetConstructors()
                .Where(ctor => ctor.GetParameters().Length == 1);

            // Look for a public constructor that accepts a single string parameter.
            ConstructorInfo stringCtor = ctors
                .SingleOrDefault(ctor => ctor.GetParameters()[0].ParameterType == typeof(string));
            if (stringCtor != null)
                return value => stringCtor.Invoke(new object[] { value });

            // Look for a public constructor that accepts a single object parameter.
            ConstructorInfo objectCtor = ctors
                .SingleOrDefault(ctor => ctor.GetParameters()[0].ParameterType == typeof(object));
            if (objectCtor != null)
                return value => objectCtor.Invoke(new object[] { value });

            //TODO: Should we also look for factory methods on the type? Static methods that accept a single parameter and return an instance of the type.

            if (arg is Option option)
            {
                throw new ParserException(-1,
                    $"Unable to determine an adequate way to convert parameters of {option.Name} option to type {Type.FullName}. Please specify a converter delegate for this option.");
            }
            else if (arg is Argument argument)
            {
                //TODO: Replace argument.Order with argument.Index in the error message below
                throw new ParserException(-1,
                    $"Unable to determine an adequate way to convert the argument at index {argument.Order} to type {Type.FullName}. Please specify a converter delegate for this argument.");
            }
            else
                throw new InvalidOperationException($"Unexpected arg type - {arg.GetType().FullName}");
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
        ///     Gets or sets a value indicating whether the <see cref="Value"/> property is assigned.
        /// </summary>
        internal bool Assigned { get; set; }

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
            return (IList)Activator.CreateInstance(listType, capacity);
        }
    }
}
