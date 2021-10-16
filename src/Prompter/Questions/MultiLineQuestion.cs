// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.Prompter.Questions
{
    public class MultiLineQuestion<TValue> : TextEntryQuestion<TValue>
    {
        public MultiLineQuestion(string name, Factory<string> message)
            : base(name, message)
        {
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            //TODO:
            return string.Empty;
        }
    }

    public sealed class MultiLineQuestion : MultiLineQuestion<string>
    {
        public MultiLineQuestion(string name, Factory<string> message)
            : base(name, message)
        {
        }
    }
}
