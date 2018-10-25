#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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
using System.Linq;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    public sealed class Prompter
    {
        public IReadOnlyList<IQuestion> Questions { get; }

        public Prompter(params IQuestion[] questions)
        {
            if (questions == null)
                throw new ArgumentNullException(nameof(questions));
            if (questions.Length == 0)
                throw new ArgumentException("Specify at least one question.", nameof(questions));
            Questions = questions;
        }

        public Prompter(IEnumerable<IQuestion> questions)
        {
            if (questions == null)
                throw new ArgumentNullException(nameof(questions));
            if (!questions.Any())
                throw new ArgumentException("Specify at least one question.", nameof(questions));
            Questions = questions.ToList();
        }

        public Answers Ask()
        {
            var answers = new Answers(Questions.Count);

            foreach (IQuestion question in Questions)
            {
                if (question is StaticText)
                {
                    ColorString staticText = question.StaticTextFn.Resolve(answers);
                    if (staticText != null)
                        ConsoleEx.WriteLineColor(staticText);
                    continue;
                }

                object answer = null;

                if (question.CanAskFn != null && !question.CanAskFn(answers))
                {
                    if (question.DefaultValueFn != null)
                        answer = question.DefaultValueFn(answers);
                    answers.Add(question.Name, answer);
                    continue;
                }

                bool validAnswer = false;
                do
                {
                    string input = question.AskerFn(question, answers);

                    bool optional = question.OptionalFn.Resolve(answers);
                    if (!optional && string.IsNullOrWhiteSpace(input))
                        continue;

                    if (question.RawValueValidatorFn != null)
                    {
                        ValidationResult validationResult = question.RawValueValidatorFn(input, answers);
                        if (!validationResult.Valid)
                        {
                            if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                                ConsoleEx.WriteLineColor($"[red]{validationResult.ErrorMessage}");
                            continue;
                        }
                    }

                    answer = question.TransformerFn != null ? question.TransformerFn(input) : input;

                    if (optional && string.IsNullOrWhiteSpace(input) && question.DefaultValueFn != null)
                        answer = question.DefaultValueFn(answers);

                    if (question.ValidatorFn != null)
                    {
                        ValidationResult validationResult = question.ValidatorFn(answer, answers);
                        if (!validationResult.Valid)
                        {
                            if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                                ConsoleEx.WriteLineColor($"[red]{validationResult.ErrorMessage}");
                            continue;
                        }
                    }

                    validAnswer = true;
                } while (!validAnswer);

                answers.Add(question.Name, answer);
            }

            return answers;
        }

        public static dynamic Ask(params IQuestion[] questions)
        {
            var prompter = new Prompter(questions);
            return prompter.Ask();
        }
    }
}