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

namespace ConsoleFx.Prompter.Questions
{
    public sealed class TypedQuestion<T> : Question
    {
        internal TypedQuestion(Question question)
            : base(question?.Name, question != null ? question.Message : (string)null)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));
            AskerFn = question.AskerFn;
            CanAskFn = question.CanAskFn;
            Validator = question.Validator;
            ConvertedValueValidator = question.ConvertedValueValidator;
        }

        public TypedQuestion<T> Validate(Func<T, bool> validator)
        {
            ConvertedValueValidator = (value, _) => validator((T)value);
            return this;
        }

        public TypedQuestion<T> Validate(Func<T, dynamic, bool> validator)
        {
            ConvertedValueValidator = (value, ans) => validator((T)value, ans);
            return this;
        }

        internal override AskerFn AskerFn { get; }
    }
}
