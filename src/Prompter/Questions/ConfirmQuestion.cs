// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using Spectre.Console;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class ConfirmQuestion : Question<bool, bool>
    {
        private readonly bool _default;

        public ConfirmQuestion(string name, Factory<string> message, bool @default)
            : base(name, message)
        {
            _default = @default;
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            return (bool)AnsiConsole.Confirm(Message.Resolve(answers), _default);
        }
    }
}
