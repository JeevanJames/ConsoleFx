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

namespace ConsoleFx.CmdLine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents a collection of <see cref="T:ConsoleFx.CmdLineArgs.Argument" /> objects.
    /// </summary>
    public sealed class Arguments : Args<Argument>
    {
        protected override void InsertItem(int index, Argument item)
        {
            base.InsertItem(index, item);
            VerifyOptionalArgumentsAtEnd();
        }

        protected override void SetItem(int index, Argument item)
        {
            base.SetItem(index, item);
            VerifyOptionalArgumentsAtEnd();
        }

        protected override string GetDuplicateErrorMessage(string name)
        {
            return string.Format(Errors.Arguments_Duplicate_argument, name);
        }

        /// <summary>
        ///     Called whenever an argument is added or set in the collection to verify that optional arguments are
        ///     specified only after the required ones.
        /// </summary>
        private void VerifyOptionalArgumentsAtEnd()
        {
            //TODO: Try and optimize this to not traverse the whole list each time.
            var inOptionalSet = false;
            foreach (var argument in this)
            {
                if (inOptionalSet)
                {
                    if (!argument.IsOptional)
                    {
                        throw new ParserException(ParserException.Codes.RequiredArgumentsDefinedAfterOptional,
                            Messages.RequiredArgumentsDefinedAfterOptional);
                    }
                }
                else
                {
                    inOptionalSet = argument.IsOptional;
                }
            }
        }
    }
}
