#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public class PasswordQuestion<TValue> : TextEntryQuestion<TValue>
    {
        private readonly AskerFn _askerFn;
        private bool _hideCursor;
        private bool _hideMask;

        internal PasswordQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
            _askerFn = (q, ans) =>
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

        internal override AskerFn AskerFn => _askerFn;
    }

    public sealed class PasswordQuestion : PasswordQuestion<string>
    {
        internal PasswordQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }
    }
}
