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

namespace ConsoleFx.Prompter
{
    public sealed class Prompter
    {
        public Prompter(params Question[] questions)
        {
            if (questions == null)
                throw new ArgumentNullException(nameof(questions));
            if (questions.Length == 0)
                throw new ArgumentException("Specify at least one question.", nameof(questions));
            Questions = questions;
        }

        public Prompter(IEnumerable<Question> questions)
        {
            if (questions == null)
                throw new ArgumentNullException(nameof(questions));
            if (!questions.Any())
                throw new ArgumentException("Specify at least one question.", nameof(questions));
            Questions = questions.ToList();
        }

        public IReadOnlyList<Question> Questions { get; }

        public Answers Ask()
        {
            var answers = new Answers(Questions.Count);

            foreach (Question question in Questions)
            {
                object answer = null;

                if (!question.CanAsk(answers))
                {
                    answer = question.DefaultValue.Resolve(answers);
                    if (answer != null)
                        answers.Add(question.Name, answer);
                    continue;
                }

                if (question is StaticText)
                {
                    question.AskerFn(question, answers);
                    continue;
                }

                bool validAnswer = false;
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
                } while (!validAnswer);

                answers.Add(question.Name, answer);
            }

            return answers;
        }

        public static dynamic Ask(params Question[] questions)
        {
            var prompter = new Prompter(questions);
            return prompter.Ask();
        }
    }
}