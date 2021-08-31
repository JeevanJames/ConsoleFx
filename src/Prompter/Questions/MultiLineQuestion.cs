// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public class MultiLineQuestion<TValue> : TextEntryQuestion<TValue>
    {
        public MultiLineQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            ConsoleEx.PrintLine(new ColorString(Message.Resolve(answers),
                PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));
            return string.Empty;
        }
    }

    public sealed class MultiLineQuestion : MultiLineQuestion<string>
    {
        public MultiLineQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }
    }
}
