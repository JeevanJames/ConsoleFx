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

        internal PasswordQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
            AskerFn = (q, ans) =>
            {
                var pq = (PasswordQuestion<TValue>)q;
                return ConsoleEx.ReadSecret(new ColorString(q.Message.Resolve(ans),
                    PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor),
                    hideCursor: pq._hideCursor, hideMask: pq._hideMask);
            };
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

        internal override AskerFn AskerFn { get; }
    }

    public sealed class PasswordQuestion : PasswordQuestion<string>
    {
        public PasswordQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }
    }
}
