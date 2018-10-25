﻿#region --- License & Copyright Notice ---
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
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    public sealed partial class Question : IQuestion
    {
        private readonly FunctionOrValue<string> _message;
        private FunctionOrValue<bool> _optional;
        private AnswersFunc<bool> _canAsk;
        private Validator<string> _rawValueValidator;
        private Func<string, object> _transformer;
        private readonly AskerFn _asker;

        public string Name { get; }

        FunctionOrValue<string> IQuestion.MessageFn => _message;

        FunctionOrValue<bool> IQuestion.OptionalFn => _optional;

        AnswersFunc<bool> IQuestion.CanAskFn => _canAsk;

        FunctionOrValue<ColorString> IQuestion.StaticTextFn => throw new NotImplementedException();

        AnswersFunc<object> IQuestion.DefaultValueFn => null;

        Validator<string> IQuestion.RawValueValidatorFn => _rawValueValidator;

        Func<string, object> IQuestion.TransformerFn => _transformer;

        Validator<object> IQuestion.ValidatorFn => null;

        AskerFn IQuestion.AskerFn => _asker;

        internal Question(string name, FunctionOrValue<string> message, AskerFn asker)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Question name must be specified.", nameof(name));
            Name = name;
            _message = message;
            _asker = asker;
        }

        public Question Optional()
        {
            _optional = true;
            return this;
        }

        public Question Optional(FunctionOrValue<bool> optional)
        {
            _optional = optional;
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

    public sealed partial class Question
    {
        public static Question Input(string name, FunctionOrValue<string> message)
        {
            AskerFn asker = (q, ans) => ConsoleEx.Prompt(new ColorString().Cyan(q.MessageFn.Resolve(ans)));
            return new Question(name, message, asker);
        }

        public static Question Password(string name, FunctionOrValue<string> message)
        {
            AskerFn asker = (q, ans) => ConsoleEx.ReadSecret(new ColorString().Cyan(q.MessageFn.Resolve(ans)));
            return new Question(name, message, asker);
        }

        public static Question<bool> Confirm(string name, FunctionOrValue<string> message)
        {
            AskerFn asker = (q, ans) =>
            {
                ConsoleEx.WriteColor(q.MessageFn.Resolve(ans) + "(y/n)");
                ConsoleKey pressed = ConsoleEx.WaitForKeys(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.Enter);
                ConsoleEx.WriteBlankLine();
                return pressed.ToString();
            };
            return new Question(name, message, asker)
                .Transform(str => str == ConsoleKey.Enter.ToString() || str == ConsoleKey.Y.ToString());
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
        private AskerFn _asker;

        public string Name { get; }

        FunctionOrValue<string> IQuestion.MessageFn => _message;

        FunctionOrValue<bool> IQuestion.OptionalFn => _mustAnswer;

        AnswersFunc<bool> IQuestion.CanAskFn => _canAsk;

        FunctionOrValue<ColorString> IQuestion.StaticTextFn => throw new NotImplementedException();

        AnswersFunc<object> IQuestion.DefaultValueFn => _defaultValueGetter;

        Validator<string> IQuestion.RawValueValidatorFn => _rawValueValidator;

        Func<string, object> IQuestion.TransformerFn => _transformer;

        Validator<object> IQuestion.ValidatorFn => _validator;

        AskerFn IQuestion.AskerFn => _asker;

        internal Question(IQuestion question)
        {
            Name = question.Name;
            _message = question.MessageFn;
            _mustAnswer = question.OptionalFn;
            _canAsk = question.CanAskFn;
            _defaultValueGetter = question.DefaultValueFn;
            _rawValueValidator = question.RawValueValidatorFn;
            _transformer = question.TransformerFn;
            _validator = question.ValidatorFn;
            _asker = question.AskerFn;
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