// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using Spectre.Console;

namespace ConsoleFx.Prompter.Questions
{
    public class ListQuestion<TValue> : Question<int, TValue>
    {
        private readonly IReadOnlyList<string> _choices;

        public ListQuestion(string name, Factory<string> message, IEnumerable<string> choices)
            : base(name, message)
        {
            if (choices is null)
                throw new ArgumentNullException(nameof(choices));

            _choices = choices.ToList();
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            string choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title((string)Message.Resolve(answers))
                .AddChoices(_choices));

            for (int i = 0; i < _choices.Count; i++)
            {
                if (string.Equals(choice, _choices[i], StringComparison.Ordinal))
                    return i;
            }

            return -1;
        }
    }

    public sealed class ListQuestion : ListQuestion<int>
    {
        public ListQuestion(string name, Factory<string> message, IEnumerable<string> choices)
            : base(name, message, choices)
        {
        }
    }
}
