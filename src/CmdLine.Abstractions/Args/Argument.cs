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
using System.Diagnostics;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents a non-option command-line parameter.
    /// </summary>
    [DebuggerDisplay("Argument {Name} [{Validators.Count} validators]")]
    public sealed class Argument : ArgumentOrOption<Argument>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ValidatorCollection _validators;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Argument" /> class.
        /// </summary>
        /// <param name="name">The unique name identifying the argument.</param>
        /// <param name="isOptional">Indicates whether the argument is optional.</param>
        /// <param name="maxOccurences">
        ///     Maximum number of occurences of the last argument. Ignored if it is not the last
        ///     argument.
        /// </param>
        public Argument(string name, bool isOptional = false, byte maxOccurences = 1)
            : base(new Dictionary<string, bool> { [name] = false })
        {
            if (maxOccurences < 1)
                throw new ArgumentException($"Maximum occurences for the {name} argument should not be less than one.", nameof(maxOccurences));

            IsOptional = isOptional;
            MaxOccurences = maxOccurences;
        }

        /// <summary>
        ///     Gets a value indicating whether the argument is optional. Optional arguments can only
        ///     be specified after all the required arguments.
        /// </summary>
        public bool IsOptional { get; }

        public int MaxOccurences { get; }

        /// <summary>
        ///     Gets the validators for this argument.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal ValidatorCollection Validators => _validators ?? (_validators = new ValidatorCollection());

        public override Argument UnderGroups(params int[] groups)
        {
            InternalUnderGroups(groups);
            return this;
        }

        public override Argument DefaultsTo(Func<object> setter)
        {
            InternalDefaultsTo(setter);
            return this;
        }

        public override Argument DefaultsTo(object defaultValue)
        {
            InternalDefaultsTo(defaultValue);
            return this;
        }

        /// <inheritdoc />
        public override Argument FormatAs(Func<string, string> formatter)
        {
            InternalFormatAs(formatter);
            return this;
        }

        public override Argument FormatAs(string formatStr)
        {
            InternalFormatAs(formatStr);
            return this;
        }

        public override Argument TypeAs(Type type, Converter<string, object> converter = null)
        {
            InternalTypeAs(type, converter);
            return this;
        }

        /// <summary>
        ///     Specifies the type to convert the option parameters, with an optional custom converter.
        ///     If a custom converter is not specified, the type's type converter will be used.
        /// </summary>
        /// <typeparam name="T">The type to convert the option parameters to.</typeparam>
        /// <param name="converter">Optional custom converter.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public override Argument TypeAs<T>(Converter<string, T> converter = null)
        {
            InternalTypeAs<T>(typeof(T), converter);
            return this;
        }

        /// <summary>
        ///     Specifies one or more validators to validate the argument.
        /// </summary>
        /// <param name="validators">The validators to use to validate the argument.</param>
        /// <returns>The same instance of the <see cref="Argument"/> object to allow for fluent syntax.</returns>
        public override Argument ValidateWith(params Validator[] validators)
        {
            foreach (Validator validator in validators)
                Validators.Add(validator);
            return this;
        }
    }
}
