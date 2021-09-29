// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents a collection of <see cref="Argument" /> objects.
    /// </summary>
    public sealed class Arguments : Args<Argument>
    {
        protected override void InsertItem(int index, Argument item)
        {
            base.InsertItem(index, item);
            VerifyOptionalArgumentsAtEnd(index);
        }

        protected override void SetItem(int index, Argument item)
        {
            base.SetItem(index, item);
            VerifyOptionalArgumentsAtEnd(index);
        }

        /// <summary>
        ///     Called whenever an argument is added or set in the collection to verify that optional arguments are
        ///     specified only after the required ones.
        /// </summary>
        private void VerifyOptionalArgumentsAtEnd(int index)
        {
            // We don't need to traverse the entire list. Assuming that the existing list is already
            // in the correct order, we just need to start traversing from the item before the specified
            // index.
            int startIndex = index - 1 >= 0 ? index - 1 : 0;

            bool inOptionalSet = false;

            for (int i = startIndex; i < Count; i++)
            {
                Argument argument = this[i];

                // If we're iterating over optional arguments and the current argument is not optional,
                // throw an exception.
                if (inOptionalSet)
                {
                    if (!argument.IsOptional)
                    {
                        throw new ParserException(ParserException.Codes.RequiredArgumentsDefinedAfterOptional,
                            Messages.RequiredArgumentsDefinedAfterOptional);
                    }
                }
                else
                    inOptionalSet = argument.IsOptional;
            }
        }
    }
}
