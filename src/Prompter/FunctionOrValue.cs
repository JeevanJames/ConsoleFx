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
            return Function != null ? (TValue)Function(answers) : Value;
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
