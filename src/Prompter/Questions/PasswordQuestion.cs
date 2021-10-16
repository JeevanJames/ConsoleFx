// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public class PasswordQuestion<TValue> : TextEntryQuestion<TValue>
    {
        private bool _hideCursor;
        private bool _hideMask;

        internal PasswordQuestion(string name, Factory<string> message)
            : base(name, message)
        {
        }

        public PasswordQuestion<TValue> HideCursor()
        {
            _hideCursor = true;
            return this;
        }

        public PasswordQuestion<TValue> HideMask()
        {
            _hideMask = true;
            return this;
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            return ConsoleEx.ReadSecret(Message.Resolve(answers), _hideCursor, _hideMask);
        }
    }

    public sealed class PasswordQuestion : PasswordQuestion<string>
    {
        public PasswordQuestion(string name, Factory<string> message)
            : base(name, message)
        {
        }
    }
}
