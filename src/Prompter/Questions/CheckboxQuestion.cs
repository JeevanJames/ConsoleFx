// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class CheckboxQuestion : Question<IReadOnlyList<int>, IReadOnlyList<int>>
    {
        private readonly IReadOnlyList<string> _choices;

        public CheckboxQuestion(string name, FunctionOrColorString message, IEnumerable<string> choices)
            : base(name, message)
        {
            if (choices is null)
                throw new ArgumentNullException(nameof(choices));
            _choices = choices.ToList();

            AskerFn = (q, ans) =>
            {
                var cq = (CheckboxQuestion)q;
                ConsoleEx.PrintLine(new ColorString(q.Message.Resolve(ans),
                    PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));
                return ConsoleEx.SelectMultiple(cq._choices);
            };
        }

        internal override AskerFn AskerFn { get; }
    }
}
