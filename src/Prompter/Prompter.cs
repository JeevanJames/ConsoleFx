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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    public sealed partial class Prompter
    {
        public Answers Ask()
        {
            var answers = new Answers(_promptItems.Count);

            EventHandler<BeforeAfterPromptEventArgs> beforePrompt = BeforePrompt;
            EventHandler<BetweenPromptEventArgs> betweenPrompts = BetweenPrompts;
            EventHandler<BeforeAfterPromptEventArgs> afterPrompt = AfterPrompt;

            for (int i = 0; i < _promptItems.Count; i++)
            {
                PromptItem promptItem = _promptItems[i];

                beforePrompt?.Invoke(this, new BeforeAfterPromptEventArgs { Prompt = promptItem });

                object answer;

                // If the prompt cannot be displayed, continue the loop.
                // If it is a question, try assigning the default value, if available, before
                // continuing.
                if (!promptItem.CanAsk(answers))
                {
                    if (promptItem is Question q)
                    {
                        answer = q.DefaultValue.Resolve(answers);
                        if (answer != null)
                            answers.Add(q.Name, answer);
                    }

                    continue;
                }

                // If the prompt is static text, just display it and continue the loop.
                if (promptItem is StaticText)
                {
                    promptItem.AskerFn(promptItem, answers);
                    continue;
                }

                var question = promptItem as Question;

                if (question.Instructions.Count > 0)
                {
                    foreach (FunctionOrValue<string> instruction in question.Instructions)
                    {
                        var cstr = new ColorString().Text(instruction.Resolve(answers),
                            Style.Instructions.ForeColor, Style.Instructions.BackColor);
                        ConsoleEx.PrintLine(cstr);
                    }
                }

                answer = null;
                bool validAnswer = false;
                do
                {
                    object input = question.AskerFn(question, answers);

                    if (question.RawValueValidator != null)
                    {
                        ValidationResult validationResult = question.RawValueValidator(input, answers);
                        if (!validationResult.Valid)
                        {
                            if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                                ConsoleEx.PrintLine($"{Clr.Red}{validationResult.ErrorMessage}");
                            continue;
                        }
                    }

                    if (input is null || (input is string s && s.Length == 0))
                        answer = question.DefaultValue.Resolve(answers);
                    else
                        answer = input;

                    answer = question.Convert(answer);

                    if (question.ConvertedValueValidator != null)
                    {
                        ValidationResult validationResult = question.ConvertedValueValidator(answer, answers);
                        if (!validationResult.Valid)
                        {
                            if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                                ConsoleEx.PrintLine($"{Clr.Red}{validationResult.ErrorMessage}");
                            continue;
                        }
                    }

                    validAnswer = true;
                }
#pragma warning disable S2583 // Conditionally executed blocks should be reachable
                while (!validAnswer);
#pragma warning restore S2583 // Conditionally executed blocks should be reachable

                answers.Add(question.Name, answer);

                if (i < _promptItems.Count - 1)
                {
                    betweenPrompts?.Invoke(this, new BetweenPromptEventArgs
                    {
                        PreviousPrompt = promptItem,
                        NextPrompt = _promptItems[i + 1],
                    });
                }

                afterPrompt?.Invoke(this, new BeforeAfterPromptEventArgs { Prompt = promptItem });
            }

            return answers;
        }

        public event EventHandler<BeforeAfterPromptEventArgs> BeforePrompt;

        public event EventHandler<BetweenPromptEventArgs> BetweenPrompts;

        public event EventHandler<BeforeAfterPromptEventArgs> AfterPrompt;

        private static Styling _style;

        public static Styling Style
        {
            get => _style ?? (_style = Styling.NoTheme);
            set => _style = value;
        }
    }

    // IList<PromptItem> implementation of Prompter.
    public sealed partial class Prompter : IList<PromptItem>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<PromptItem> _promptItems = new List<PromptItem>();

        public PromptItem this[int index]
        {
            get => _promptItems[index];
            set => _promptItems[index] = value;
        }

        public int Count => _promptItems.Count;

        public bool IsReadOnly => ((IList<PromptItem>)_promptItems).IsReadOnly;

        public void Add(PromptItem item)
        {
            _promptItems.Add(item);
        }

        public void Clear()
        {
            _promptItems.Clear();
        }

        public bool Contains(PromptItem item)
        {
            return _promptItems.Contains(item);
        }

        public void CopyTo(PromptItem[] array, int arrayIndex)
        {
            _promptItems.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PromptItem> GetEnumerator()
        {
            return ((IList<PromptItem>)_promptItems).GetEnumerator();
        }

        public int IndexOf(PromptItem item)
        {
            return _promptItems.IndexOf(item);
        }

        public void Insert(int index, PromptItem item)
        {
            _promptItems.Insert(index, item);
        }

        public bool Remove(PromptItem item)
        {
            return _promptItems.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _promptItems.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<PromptItem>)_promptItems).GetEnumerator();
        }
    }

    public sealed class BetweenPromptEventArgs : EventArgs
    {
        public PromptItem PreviousPrompt { get; set; }

        public PromptItem NextPrompt { get; set; }
    }

    public sealed class BeforeAfterPromptEventArgs : EventArgs
    {
        public PromptItem Prompt { get; set; }
    }
}
