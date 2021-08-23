// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public class MultiLineQuestion<TValue> : TextEntryQuestion<TValue>
    {
        private readonly AskerFn _askerFn;

        public MultiLineQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
            _askerFn = (q, ans) =>
            {
                ConsoleEx.PrintLine(new ColorString(q.Message.Resolve(ans),
                    PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));
                return string.Empty;
            };
        }

        internal override AskerFn AskerFn => _askerFn;
    }

    public sealed class MultiLineQuestion : MultiLineQuestion<string>
    {
        public MultiLineQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }
    }
}
