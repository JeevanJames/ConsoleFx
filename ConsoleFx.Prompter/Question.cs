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

namespace ConsoleFx.Prompter
{
    public sealed class Question
    {
        public string Name { get; }

        public string Message { get; }

        public bool MustAnswer { get; }

        internal Func<dynamic, bool> WhenFn { get; set; }

        internal Func<dynamic, object> DefaultValueGetter { get; set; }

        internal Func<string, object> Transformer { get; set; }

        internal Func<string, dynamic, bool> RawValueValidator { get; set; }

        internal Func<object, dynamic, bool> Validator { get; set; }

        internal Question(string name, string message, bool mustAnswer = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Question name must be specified.", nameof(name));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Question message must be specified.", nameof(message));
            Name = name;
            Message = message;
            MustAnswer = mustAnswer;
        }

        public static Question Optional(string name, string message) =>
            new Question(name, message);

        public static Question Optional(string name, string message, object defaultValue) =>
            new Question(name, message, mustAnswer: false)
                .DefaultValue(ans => defaultValue);

        public static Question Mandatory(string name, string message) =>
            new Question(name, message, mustAnswer: true);

        public Question When(Func<dynamic, bool> when)
        {
            if (when == null)
                throw new ArgumentNullException(nameof(when));
            if (WhenFn != null)
                throw new ArgumentException($"When condition is already defined for the question {Name}.", nameof(when));
            WhenFn = when;
            return this;
        }

        public Question DefaultValue(Func<dynamic, object> defaultValueGetter)
        {
            if (defaultValueGetter == null)
                throw new ArgumentNullException(nameof(defaultValueGetter));
            if (DefaultValueGetter != null)
                throw new ArgumentException($"Default value getter is alread defined for the question {Name}.", nameof(defaultValueGetter));
            DefaultValueGetter = defaultValueGetter;
            return this;
        }

        public Question Transform<T>(Func<string, T> transformer)
        {
            if (transformer == null)
                throw new ArgumentNullException(nameof(transformer));
            if (Transformer != null)
                throw new ArgumentException($"Transformer is already defined for the question {Name}.", nameof(transformer));
            Transformer = value => (object)transformer(value);
            return this;
        }

        public Question ValidateRawValue(Func<string, dynamic, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (RawValueValidator != null)
                throw new ArgumentException($"Raw value validator is already defined for the question {Name}.", nameof(validator));
            RawValueValidator = validator;
            return this;
        }

        public Question Validate<TValue>(Func<TValue, dynamic, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (Validator != null)
                throw new ArgumentException($"Validator is already defined for the question {Name}.", nameof(validator));
            Validator = (value, answers) => validator((TValue)value, answers);
            return this;
        }
    }
}