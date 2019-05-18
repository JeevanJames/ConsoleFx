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

using ConsoleFx.CmdLineArgs.Validators.Bases;

namespace ConsoleFx.CmdLineArgs.Bases
{
    public abstract class ArgumentOrOption<TArg> : Arg
        where TArg : Arg
    {
        protected ArgumentOrOption()
        {
        }

        protected ArgumentOrOption(IDictionary<string, bool> names)
            : base(names)
        {
        }

        /// <summary>
        ///     Gets or sets the optional delegate to return the arg's default value, if it is not set.
        /// </summary>
        public Func<object> DefaultSetter { get; set; }

        /// <summary>
        ///     Gets or sets the optional delegate that allows an arg value to be custom formatted.
        ///     <para />
        ///     During parsing, the formatting is performed before any type conversion.
        /// </summary>
        public Func<string, string> Formatter { get; set; }

        /// <summary>
        ///     Gets or sets the type that the arg's values should be converted to. If a
        ///     <see cref="TypeConverter" /> is specified, then it is used to perform the type
        ///     conversion, otherwise the framework looks for a default string-to-type type converter
        ///     for the expected type. If a type converter is not found, an exception is thrown during
        ///     the parsing.
        /// </summary>
        /// <remarks>
        ///     In the case of <see cref="Option"/>, this type applies to all parameters. In case
        ///     different parameters are to be converted to different types, the conversion must happen
        ///     outside the ConsoleFx framework.
        /// </remarks>
        public Type Type { get; set; }

        /// <summary>
        ///     Gets or sets the optional converter to convert a string arg value to the actual
        ///     <see cref="Type" />.
        /// </summary>
        public Converter<string, object> TypeConverter { get; set; }

        public abstract TArg DefaultsTo(Func<object> setter);

        public abstract TArg DefaultsTo(object defaultValue);

        /// <summary>
        ///     Assigns the <see cref="Formatter"/> property, which can be used to format the arg's
        ///     value.
        /// </summary>
        /// <param name="formatter">The formatter delegate.</param>
        /// <returns>The instance of the <typeparamref name="TArg"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the formatter delegate is <c>null</c>.
        /// </exception>
        public abstract TArg FormatAs(Func<string, string> formatter);

        /// <summary>
        ///     Assigns a format string to the arg, which will be used to format the arg's value.
        /// </summary>
        /// <param name="formatStr">
        ///     The format string, where the first format placeholder ({0}) represents the arg's value.
        /// </param>
        /// <returns>The instance of the <typeparamref name="TArg"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the format string is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if the format string does not contain a format placeholder ({0}).
        /// </exception>
        public abstract TArg FormatAs(string formatStr);

        public abstract TArg TypedAs(Type type, Converter<string, object> converter = null);

        /// <summary>
        ///     Specifies the type to convert the option parameters, with an optional custom converter.
        ///     If a custom converter is not specified, the type's type converter will be used.
        /// </summary>
        /// <typeparam name="T">The type to convert the option parameters to.</typeparam>
        /// <param name="converter">Optional custom converter.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public abstract TArg TypedAs<T>(Converter<string, T> converter = null);

        /// <summary>
        ///     Specifies one or more validators that will be used to validate the arg's values.
        /// </summary>
        /// <param name="validators">One or more validators.</param>
        /// <returns>The instance of the <typeparamref name="TArg"/>.</returns>
        public abstract TArg ValidateWith(params Validator[] validators);

        protected void InternalDefaultsTo(Func<object> setter)
        {
            if (setter is null)
                throw new ArgumentNullException(nameof(setter));
            DefaultSetter = setter;
        }

        protected void InternalDefaultsTo(object value)
        {
            DefaultSetter = () => value;
        }

        protected void InternalFormatAs(Func<string, string> formatter)
        {
            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));
            Formatter = formatter;
        }

        protected void InternalFormatAs(string formatStr)
        {
            if (formatStr is null)
                throw new ArgumentNullException(nameof(formatStr));
            if (!formatStr.Contains("{0}"))
                throw new ArgumentException(Errors.Option_MissingPlaceholderInFormatString, nameof(formatStr));

            //TODO: Should we handle null values?
            Formatter = value => string.Format(formatStr, value);
        }

        protected void InternalTypedAs<T>(Type type, Converter<string, T> converter = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
            if (converter != null)
                TypeConverter = value => converter(value);
        }
    }
}
