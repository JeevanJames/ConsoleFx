// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.Prompter.Questions
{
    public abstract class TextEntryQuestion<TValue> : Question<string, TValue>
    {
        protected TextEntryQuestion(string name, Factory<string> message)
            : base(name, message)
        {
        }

        public TextEntryQuestion<TValue> Required(bool allowWhitespaceOnly = false)
        {
            IsRequired = true;
            AllowWhitespaceOnly = allowWhitespaceOnly;
            return this;
        }

        internal bool IsRequired { get; set; }

        internal bool AllowWhitespaceOnly { get; set; }
    }
}
