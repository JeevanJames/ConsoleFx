#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public class ListQuestion<TValue> : Question<int, TValue>
    {
        private readonly IReadOnlyList<string> _choices;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly AskerFn _askerFn;

        public ListQuestion(string name, FunctionOrValue<string> message, IEnumerable<string> choices)
            : base(name, message)
        {
            if (choices is null)
                throw new ArgumentNullException(nameof(choices));

            _choices = choices.ToList();

            _askerFn = (q, ans) =>
            {
                var lq = (ListQuestion<TValue>)q;
                ConsoleEx.PrintLine(q.Message.Resolve(ans));
                return ConsoleEx.SelectSingle(lq._choices);
            };
        }

        internal override AskerFn AskerFn => _askerFn;
    }

    public sealed class ListQuestion : ListQuestion<int>
    {
        public ListQuestion(string name, FunctionOrValue<string> message, IEnumerable<string> choices)
            : base(name, message, choices)
        {
        }
    }
}
