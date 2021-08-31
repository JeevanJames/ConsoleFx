// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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

        public ListQuestion(string name, FunctionOrColorString message, IEnumerable<string> choices)
            : base(name, message)
        {
            if (choices is null)
                throw new ArgumentNullException(nameof(choices));

            _choices = choices.ToList();
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            ConsoleEx.PrintLine(new ColorString(Message.Resolve(answers),
                PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));
            return ConsoleEx.SelectSingle(_choices);
        }
    }

    public sealed class ListQuestion : ListQuestion<int>
    {
        public ListQuestion(string name, FunctionOrColorString message, IEnumerable<string> choices)
            : base(name, message, choices)
        {
        }
    }
}
