#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ConsoleFx.Parser
{
    /// <summary>
    /// Represents a non-option command-line parameter.
    /// </summary>
    [DebuggerDisplay("Argument '{Name ?? string.Empty}' [{Validators.Count} validators]")]
    public sealed class Argument
    {
        public string Name { get; set; }

        /// <summary>
        /// Delegate to call when the argument is encountered.
        /// </summary>
        public ArgumentHandler Handler { get; set; }

        /// <summary>
        /// Indicates whether the argument is optional. Like C# optional parameters, then can only
        /// be specified after all the required parameters.
        /// </summary>
        public bool IsOptional { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ValidatorCollection Validators { get; } = new ValidatorCollection();

        internal object Scope { get; set; }
    }

    public delegate void ArgumentHandler(string value);

    public sealed class Arguments : Collection<Argument>
    {
        public Arguments()
        {
        }

        public Arguments(IList<Argument> list)
            : base(list)
        {
        }

        public Arguments(IEnumerable<Argument> arguments)
        {
            foreach (Argument argument in arguments)
                Add(argument);
        }

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

        //Called whenever an argument is added or set in the collection.
        //Verifies that the optional arguments are specified only after the required ones.
        //TODO: Try and optimize this to not traverse the whole list each time.
        private void VerifyOptionalArgumentsAtEnd()
        {
            bool optional = false;
            foreach (Argument argument in this)
            {
                if (!optional)
                    optional = argument.IsOptional;
                else
                {
                    if (!argument.IsOptional)
                        throw new ParserException(ParserException.Codes.RequiredArgumentsDefinedAfterOptional,
                            Messages.RequiredArgumentsDefinedAfterOptional);
                }
            }
        }
    }
}
