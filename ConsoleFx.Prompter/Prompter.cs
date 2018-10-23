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
        public IReadOnlyList<Question> Questions { get; }

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

        public Answers Ask()
        {
            var answers = new Answers(Questions.Count);

            foreach (Question question in Questions)
            {
                object answer = null;

                if (question.WhenFn != null && !question.WhenFn(answers))
                {
                    if (question.DefaultValueGetter != null)
                        answer = question.DefaultValueGetter(answers);
                    answers.Add(question.Name, answer);
                    continue;
                }

                repeat_question:
                string input = ConsoleEx.Prompt($"[cyan]{question.Message} ");

                if (question.MustAnswer && string.IsNullOrWhiteSpace(input))
                    goto repeat_question;

                if (question.RawValueValidator != null && !question.RawValueValidator(input, answers))
                    goto repeat_question;

                answer = question.Transformer != null ? question.Transformer(input) : input;

                if (!question.MustAnswer && string.IsNullOrWhiteSpace(input) && question.DefaultValueGetter != null)
                    answer = question.DefaultValueGetter(answers);

                if (question.Validator != null && !question.Validator(answer, answers))
                    goto repeat_question;

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