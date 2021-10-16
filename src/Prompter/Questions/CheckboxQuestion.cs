// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using Spectre.Console;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class CheckboxQuestion : Question<IReadOnlyList<int>, IReadOnlyList<int>>
    {
        private readonly IReadOnlyList<string> _choices;

        public CheckboxQuestion(string name, string message, IEnumerable<string> choices)
            : base(name, message)
        {
            if (choices is null)
                throw new ArgumentNullException(nameof(choices));
            _choices = choices.ToList();
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            string message = Message.Resolve(answers);
            List<string> selectedItems = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                .Title(message)
                .AddChoices(_choices));
            return selectedItems.Select(IndexOf).Where(index => index >= 0).ToList();
        }

        private int IndexOf(string str)
        {
            for (int i = 0; i < _choices.Count; i++)
            {
                if (string.Equals(_choices[i], str, StringComparison.Ordinal))
                    return i;
            }

            return -1;
        }
    }
}
