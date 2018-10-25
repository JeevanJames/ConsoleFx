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
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{

    public sealed class Question : IQuestion
    {
        private readonly FunctionOrValue<string> _message;
        private FunctionOrValue<bool> _mustAnswer;
        private AnswersFunc<bool> _canAsk;
        private Validator<string> _rawValueValidator;
        private Func<string, object> _transformer;

        public string Name { get; }

        FunctionOrValue<string> IQuestion.Message => _message;

        FunctionOrValue<bool> IQuestion.MustAnswer => _mustAnswer;

        AnswersFunc<bool> IQuestion.CanAsk => _canAsk;

        FunctionOrValue<IReadOnlyList<ColorString>> IQuestion.Banner => throw new NotImplementedException();

        AnswersFunc<object> IQuestion.DefaultValueGetter => null;

        Validator<string> IQuestion.RawValueValidator => _rawValueValidator;

        Func<string, object> IQuestion.Transformer => _transformer;

        Validator<object> IQuestion.Validator => null;

        internal Question(string name, FunctionOrValue<string> message)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Question name must be specified.", nameof(name));
            Name = name;
            _message = message;
        }

        public static Question New(string name, FunctionOrValue<string> message) =>
            new Question(name, message);

        public static Question Mandatory(string name, FunctionOrValue<string> message) =>
            new Question(name, message).MustAnswer(true);

        public static Question Optional(string name, FunctionOrValue<string> message) =>
            new Question(name, message).MustAnswer(false);

        public Question MustAnswer(FunctionOrValue<bool> mustAnswer)
        {
            _mustAnswer = mustAnswer;
            return this;
        }

        public Question When(AnswersFunc<bool> when)
        {
            if (when == null)
                throw new ArgumentNullException(nameof(when));
            if (_canAsk != null)
                throw new ArgumentException($"When condition is already defined for the question {Name}.", nameof(when));
            _canAsk = when;
            return this;
        }

        public Question Validate(Validator<string> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (_rawValueValidator != null)
                throw new ArgumentException($"Raw value validator is already defined for the question {Name}.", nameof(validator));
            _rawValueValidator = validator;
            return this;
        }

        public Question<TValue> Transform<TValue>(Func<string, TValue> transformer)
        {
            if (transformer == null)
                throw new ArgumentNullException(nameof(transformer));
            _transformer = str => transformer(str);
            return new Question<TValue>(this);
        }
    }

    public sealed class Question<TValue> : IQuestion
    {
        private readonly FunctionOrValue<string> _message;
        private FunctionOrValue<bool> _mustAnswer;
        private AnswersFunc<bool> _canAsk;
        private AnswersFunc<object> _defaultValueGetter;
        private Validator<string> _rawValueValidator;
        private Func<string, object> _transformer;
        private Validator<object> _validator;

        public string Name { get; }

        FunctionOrValue<string> IQuestion.Message => _message;

        FunctionOrValue<bool> IQuestion.MustAnswer => _mustAnswer;

        AnswersFunc<bool> IQuestion.CanAsk => _canAsk;

        FunctionOrValue<IReadOnlyList<ColorString>> IQuestion.Banner => throw new NotImplementedException();

        AnswersFunc<object> IQuestion.DefaultValueGetter => _defaultValueGetter;

        Validator<string> IQuestion.RawValueValidator => _rawValueValidator;

        Func<string, object> IQuestion.Transformer => _transformer;

        Validator<object> IQuestion.Validator => _validator;

        internal Question(IQuestion question)
        {
            Name = question.Name;
            _message = question.Message;
            _mustAnswer = question.MustAnswer;
            _canAsk = question.CanAsk;
            _defaultValueGetter = question.DefaultValueGetter;
            _rawValueValidator = question.RawValueValidator;
            _transformer = question.Transformer;
            _validator = question.Validator;
        }

        public Question<TValue> DefaultValue(AnswersFunc<TValue> defaultValueGetter)
        {
            if (defaultValueGetter == null)
                throw new ArgumentNullException(nameof(defaultValueGetter));
            _defaultValueGetter = ans => defaultValueGetter(ans);
            return this;
        }

        public Question<TValue> Validate(Func<TValue, ValidationResult> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            _validator = (value, answers) => validator((TValue) value);
            return this;
        }

        public Question<TValue> Validate(Validator<TValue> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            _validator = (value, answers) => validator((TValue)value, answers);
            return this;
        }
    }

    public static class TransformExtensions
    {
        public static Question<int> AsInteger(this Question question) =>
            question.Transform(str => int.Parse(str));
    }
}