﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

using System.Diagnostics;

using ConsoleFx.CmdLineParser.Validators;

namespace ConsoleFx.CmdLineParser
{
    /// <summary>
    ///     Represents a non-option command-line parameter.
    /// </summary>
    [DebuggerDisplay("Argument {Name} [{Validators.Count} validators]")]
    public sealed class Argument : MetadataObject
    {
        private ValidatorCollection _validators;

        public Argument(string name, bool isOptional = false) : base(name)
        {
            IsOptional = isOptional;
        }

        /// <summary>
        ///     Indicates whether the argument is optional. Like C# optional parameters, then can only
        ///     be specified after all the required arguments.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     Validators for this argument.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ValidatorCollection Validators => _validators ?? (_validators = new ValidatorCollection());

        public Argument ValidateWith(params Validator[] validators)
        {
            foreach (Validator validator in validators)
                Validators.Add(validator);
            return this;
        }
    }

    public sealed class Arguments : MetadataObjects<Argument>
    {
        protected override void InsertItem(int index, Argument argument)
        {
            base.InsertItem(index, argument);
            VerifyOptionalArgumentsAtEnd();
        }

        protected override void SetItem(int index, Argument argument)
        {
            base.SetItem(index, argument);
            VerifyOptionalArgumentsAtEnd();
        }

        protected override string GetDuplicateErrorMessage(string name) =>
            $"Argument named '{name}' already exists in the argument collection.";

        /// <summary>
        ///     Called whenever an argument is added or set in the collection to verify that
        ///     optional arguments are specified only after the required ones.
        /// </summary>
        private void VerifyOptionalArgumentsAtEnd()
        {
            //TODO: Try and optimize this to not traverse the whole list each time.
            bool inOptionalSet = false;
            foreach (Argument argument in this)
            {
                if (inOptionalSet)
                {
                    if (!argument.IsOptional)
                    {
                        throw new ParserException(ParserException.Codes.RequiredArgumentsDefinedAfterOptional,
                            Messages.RequiredArgumentsDefinedAfterOptional);
                    }
                } else
                    inOptionalSet = argument.IsOptional;
            }
        }
    }
}
