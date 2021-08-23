// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Represents a fixed value or a factory function that can generate the value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly struct FunctionOrValue<TValue>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionOrValue{TValue}"/> struct with
        ///     a fixed <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The fixed value to set.</param>
        internal FunctionOrValue(TValue value)
        {
            Value = value;
            Function = null;
            IsAssigned = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionOrValue{TValue}"/> struct with
        ///     a factory <paramref name="function"/>.
        /// </summary>
        /// <param name="function">The factory function.</param>
        internal FunctionOrValue(Func<dynamic, TValue> function)
        {
            if (function is null)
                throw new ArgumentNullException(nameof(function));
            Function = function;
            Value = default;
            IsAssigned = true;
        }

        /// <summary>
        ///     Gets the fixed value.
        /// </summary>
        internal TValue Value { get; }

        /// <summary>
        ///     Gets the factory function to generate the value.
        /// </summary>
        internal Func<dynamic, TValue> Function { get; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="FunctionOrValue{TValue}"/> is assigned
        ///     a value, i.e. whether its constructor was called.
        /// </summary>
        internal bool IsAssigned { get; }

        /// <summary>
        ///     Resolves the value of the <see cref="FunctionOrValue{TValue}"/> by either returning
        ///     the fixed value or executing the factory function and returning a dynamic value.
        /// </summary>
        /// <param name="answers">The set of answers passed to the factory function.</param>
        /// <returns>The resolved value of the <see cref="FunctionOrValue{TValue}"/>.</returns>
        internal TValue Resolve(dynamic answers = null)
        {
            if (!IsAssigned)
                throw new InvalidOperationException("The FunctionOrValue instance is not assigned.");

            if (Function is not null)
                return (TValue)Function(answers);

            bool isFunctionOrValue = ValueIsFunctionOrValue(out Type type);
            if (isFunctionOrValue)
            {
                dynamic dynamicValue = Value;
                return dynamicValue.Resolve(answers);
            }

            return Value;
        }

        /// <summary>
        ///     Implicitly converts a fixed value to a <see cref="FunctionOrValue{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The fixed value to convert.</param>
        public static implicit operator FunctionOrValue<TValue>(TValue value)
        {
            return new FunctionOrValue<TValue>(value);
        }

        /// <summary>
        ///     Implicitly converts a factory function delegate to a <see cref="FunctionOrValue{TValue}"/>
        ///     struct.
        /// </summary>
        /// <param name="function">The factory function delegate to convert.</param>
        public static implicit operator FunctionOrValue<TValue>(Func<dynamic, TValue> function)
        {
            return new FunctionOrValue<TValue>(function);
        }

        private bool ValueIsFunctionOrValue(out Type type)
        {
            Type currentType = Value.GetType();
            while (currentType != typeof(object))
            {
                if (currentType is null)
                {
                    type = null;
                    return false;
                }

                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(FunctionOrValue<>))
                {
                    type = currentType.GetGenericArguments()[0];
                    return true;
                }

                currentType = currentType.BaseType;
            }

            type = null;
            return false;
        }
    }

    public readonly struct FunctionOrColorString
    {
        internal FunctionOrColorString(ColorString cstr)
        {
            Value = cstr;
            Function = null;
            IsAssigned = true;
        }

        internal FunctionOrColorString(Func<dynamic, ColorString> function)
        {
            if (function is null)
                throw new ArgumentNullException(nameof(function));
            Function = function;
            Value = null;
            IsAssigned = true;
        }

        internal ColorString Value { get; }

        internal Func<dynamic, ColorString> Function { get; }

        internal bool IsAssigned { get; }

        internal ColorString Resolve(dynamic answers = null)
        {
            return Function != null ? Function(answers) : Value;
        }

        public static implicit operator FunctionOrColorString(ColorString cstr)
        {
            return new FunctionOrColorString(cstr);
        }

        public static implicit operator FunctionOrColorString(string str)
        {
            return new FunctionOrColorString(str);
        }

        public static implicit operator FunctionOrColorString(Func<dynamic, ColorString> function)
        {
            return new FunctionOrColorString(function);
        }
    }
}
