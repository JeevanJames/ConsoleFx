// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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

            if (converter is not null || Type == typeof(string))
                return converter;

            // Look for a type converter that can convert from string.
            TypeConverter typeConverter = TypeDescriptor.GetConverter(Type);
            if (typeConverter.CanConvertFrom(typeof(string)))
                return value => TypeDescriptor.GetConverter(Type).ConvertFromString(value);

            // Look for constructors on the type that can be used to instantiate an instance,
            // specifically those that accept a single parameter.
            IEnumerable<ConstructorInfo> ctors = Type.GetConstructors()
                .Where(ctor => ctor.GetParameters().Length == 1);

            // Look for a public constructor that accepts a single string parameter.
            ConstructorInfo stringCtor = ctors
                .SingleOrDefault(ctor => ctor.GetParameters()[0].ParameterType == typeof(string));
            if (stringCtor is not null)
                return value => stringCtor.Invoke(new object[] { value });

            // Look for a public constructor that accepts a single object parameter.
            ConstructorInfo objectCtor = ctors
                .SingleOrDefault(ctor => ctor.GetParameters()[0].ParameterType == typeof(object));
            if (objectCtor is not null)
                return value => objectCtor.Invoke(new object[] { value });

            // Look for possible static factory methods
            IEnumerable<MethodInfo> factoryMethods = Type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(mi => mi.ReturnType == Type && mi.GetParameters().Length == 1);

            // Look for a static factory method that accepts a single string parameter.
            MethodInfo stringFactory = factoryMethods
                .SingleOrDefault(mi => mi.GetParameters()[0].ParameterType == typeof(string));
            if (stringFactory is not null)
                return value => stringFactory.Invoke(null, new object[] { value });

            // Look for a static factory method that accepts a single object parameter.
            MethodInfo objectFactory = factoryMethods
                .SingleOrDefault(mi => mi.GetParameters()[0].ParameterType == typeof(object));
            if (objectFactory is not null)
                return value => objectFactory.Invoke(null, new object[] { value });

            Exception exception = arg switch
            {
                Option option => new ParserException(-1,
                    $"Unable to determine an adequate way to convert parameters of {option.Name} option to type {Type}. Please specify a converter delegate for this option."),
                Argument argument => new ParserException(-1,
                    $"Unable to determine an adequate way to convert the argument at index {argument.Order} to type {Type}. Please specify a converter delegate for this argument."),
                _ => new InvalidOperationException($"Unexpected arg type - {arg.GetType()}"),
            };

            throw exception;
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
            string formattedValue = Arg.Formatter is not null ? Arg.Formatter(rawValue) : rawValue;
            return _converter is not null ? _converter(formattedValue) : formattedValue;
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
