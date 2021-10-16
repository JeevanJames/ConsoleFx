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
    public readonly struct Factory<TValue>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Factory{TValue}"/> struct with
        ///     a fixed <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The fixed value to set.</param>
        internal Factory(TValue value)
        {
            Value = value;
            Function = null;
            IsAssigned = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Factory{TValue}"/> struct with
        ///     a factory <paramref name="function"/>.
        /// </summary>
        /// <param name="function">The factory function.</param>
        internal Factory(Func<dynamic, TValue> function)
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
        ///     Gets a value indicating whether the <see cref="Factory{TValue}"/> is assigned
        ///     a value, i.e. whether its constructor was called.
        /// </summary>
        internal bool IsAssigned { get; }

        /// <summary>
        ///     Resolves the value of the <see cref="Factory{TValue}"/> by either returning
        ///     the fixed value or executing the factory function and returning a dynamic value.
        /// </summary>
        /// <param name="answers">The set of answers passed to the factory function.</param>
        /// <returns>The resolved value of the <see cref="Factory{TValue}"/>.</returns>
        internal TValue Resolve(dynamic answers = null)
        {
            if (!IsAssigned)
                throw new InvalidOperationException("The Factory instance is not assigned.");

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
        ///     Implicitly converts a fixed value to a <see cref="Factory{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The fixed value to convert.</param>
        public static implicit operator Factory<TValue>(TValue value)
        {
            return new Factory<TValue>(value);
        }

        /// <summary>
        ///     Implicitly converts a factory function delegate to a <see cref="Factory{TValue}"/>
        ///     struct.
        /// </summary>
        /// <param name="function">The factory function delegate to convert.</param>
        public static implicit operator Factory<TValue>(Func<dynamic, TValue> function)
        {
            return new Factory<TValue>(function);
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

                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(Factory<>))
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
}
