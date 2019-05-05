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

using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.Prompter
{
    public sealed class Prompter //TODO: : IList<PromptItem>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<PromptItem> _promptItems = new List<PromptItem>();

        public IReadOnlyList<PromptItem> PromptItems => _promptItems;

        public Prompter AddItem(PromptItem promptItem)
        {
            _promptItems.Add(promptItem);
            return this;
        }

        public Answers Ask()
        {
            var answers = new Answers(PromptItems.Count);

            foreach (PromptItem promptItem in PromptItems)
            {
                object answer;

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

                if (promptItem is StaticText)
                {
                    promptItem.AskerFn(promptItem, answers);
                    continue;
                }

                var question = promptItem as Question;

                bool validAnswer;
                do
                {
                    object input = question.AskerFn(question, answers);

                    //if (question.RawValueValidatorFn != null)
                    //{
                    //    ValidationResult validationResult = question.RawValueValidatorFn(input, answers);
                    //    if (!validationResult.Valid)
                    //    {
                    //        if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                    //            ConsoleEx.PrintLine($"[red]{validationResult.ErrorMessage}");
                    //        continue;
                    //    }
                    //}
                    answer = question.Convert(input);

                    //if (answer == null || (answer as string).Length == 0)
                    //    answer = question.DefaultValue.Resolve(answers);

                    //if (optional && string.IsNullOrWhiteSpace(input) && question.DefaultValueFn != null)
                    //    answer = question.DefaultValueFn(answers);

                    //if (question.ValidatorFn != null)
                    //{
                    //    ValidationResult validationResult = question.ValidatorFn(answer, answers);
                    //    if (!validationResult.Valid)
                    //    {
                    //        if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                    //            ConsoleEx.PrintLine($"[red]{validationResult.ErrorMessage}");
                    //        continue;
                    //    }
                    //}
                    validAnswer = true;
                }
#pragma warning disable S2583 // Conditionally executed blocks should be reachable
                while (!validAnswer);
#pragma warning restore S2583 // Conditionally executed blocks should be reachable

                answers.Add(question.Name, answer);
            }

            return answers;
        }
    }
}
