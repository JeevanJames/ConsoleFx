// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using Spectre.Console;

namespace ConsoleFx.Prompter.Questions
{
    public class InputQuestion<TValue> : TextEntryQuestion<TValue>
    {
        internal InputQuestion(string name, Factory<string> message)
            : base(name, message)
        {
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            return AnsiConsole.Prompt(new TextPrompt<TValue>(Message.Resolve(answers))
                .Validate(value => ConvertedValueValidator(value, answers)));
        }
    }

    public sealed class InputQuestion : InputQuestion<string>
    {
        public InputQuestion(string name, Factory<string> message)
            : base(name, message)
        {
        }
    }
}
