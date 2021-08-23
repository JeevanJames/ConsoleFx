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
            VerifyOptionalArgumentsAtEnd();
        }

        protected override void SetItem(int index, Argument item)
        {
            base.SetItem(index, item);
            VerifyOptionalArgumentsAtEnd();
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
