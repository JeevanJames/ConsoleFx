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

namespace ConsoleFx.Prompter
{
    public abstract class Question : PromptItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        /// <param name="name">The name of the variable to store the answer.</param>
        /// <param name="message">The message to display to the user.</param>
        protected Question(string name, FunctionOrValue<string> message)
            : base(message)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify a name for the question.", nameof(name));
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the variable to store the answer.
        /// </summary>
        public string Name { get; }

        internal FunctionOrValue<object> DefaultValue { get; set; }

        internal Validator<object> RawValueValidator { get; set; }

        internal Func<object, object> ConverterFn { get; set; }

        internal Validator<object> ConvertedValueValidator { get; set; }

        internal object Convert(object value) =>
            ConverterFn != null ? ConverterFn(value) : value;
    }

    public abstract class Question<TRaw, TConverted> : Question
    {
        protected Question(string name, FunctionOrValue<string> message)
            : base(name, message)
        {
        }

        public new Question<TRaw, TConverted> When(AnswersFunc<bool> canAskFn)
        {
            CanAskFn = canAskFn;
            return this;
        }

        public Question<TRaw, TConverted> DefaultsTo(FunctionOrValue<TConverted> defaultValue)
        {
            DefaultValue = defaultValue;
            return this;
        }

        public Question<TRaw, TConverted> ValidateWith(Validator<TConverted> validator)
        {
            if (validator is null)
                throw new ArgumentNullException(nameof(validator));
            ConvertedValueValidator = (value, ans) => validator((TConverted)value, ans);
            return this;
        }

        public Question<TRaw, TConverted> ValidateInputWith(Validator<TRaw> validator)
        {
            if (validator is null)
                throw new ArgumentNullException(nameof(validator));
            RawValueValidator = (value, ans) => validator((TRaw)value, ans);
            return this;
        }

        public Question<TRaw, TConverted> Transform(Func<TRaw, TConverted> converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));
            ConverterFn = raw => converter((TRaw)raw);
            return this;
        }
    }
}
