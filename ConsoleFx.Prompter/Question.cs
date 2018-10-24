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
    public interface IQuestion
    {
        string Name { get; }

        FunctionOrValue<string> Message { get; }

        FunctionOrValue<bool> MustAnswer { get; }

        FunctionOrValue<IReadOnlyList<ColorString>> Banner { get; }

        Func<dynamic, bool> WhenFn { get; }

        Func<dynamic, object> DefaultValueGetter { get; }

        Func<string, object> Transformer { get; }

        Func<string, dynamic, bool> RawValueValidator { get; }

        Func<object, dynamic, bool> Validator { get; }
    }

    public readonly struct FunctionOrValue<TValue>
    {
        internal TValue Value { get; }

        internal Func<dynamic, TValue> Function { get; }

        internal FunctionOrValue(TValue value)
        {
            Value = value;
            Function = null;
        }

        internal FunctionOrValue(Func<dynamic, TValue> function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
            Function = function;
            Value = default;
        }

        internal TValue Resolve(dynamic answers = null) =>
            Function != null ? (TValue)Function(answers) : Value;

        public static implicit operator FunctionOrValue<TValue>(TValue value) => new FunctionOrValue<TValue>(value);

        public static implicit operator FunctionOrValue<TValue>(Func<dynamic, TValue> function) => new FunctionOrValue<TValue>(function);
    }

    public readonly struct ValidationResult
    {
        internal bool Valid { get; }

        internal string ErrorMessage { get; }

        internal ValidationResult(bool valid)
        {
            Valid = valid;
            ErrorMessage = null;
        }

        internal ValidationResult(string errorMessage)
        {
            Valid = false;
            ErrorMessage = errorMessage;
        }

        public static implicit operator ValidationResult(bool valid) =>
            new ValidationResult(valid);

        public static implicit operator ValidationResult(string errorMessage) =>
            new ValidationResult(errorMessage);
    }

    public sealed class Question : IQuestion
    {
        private readonly FunctionOrValue<string> _message;
        private FunctionOrValue<bool> _mustAnswer;

        public string Name { get; }

        FunctionOrValue<string> IQuestion.Message => _message;

        FunctionOrValue<bool> IQuestion.MustAnswer => _mustAnswer;

        FunctionOrValue<IReadOnlyList<ColorString>> IQuestion.Banner { get; } = new List<ColorString>();

        Func<dynamic, bool> IQuestion.WhenFn { get; private set; }

        Func<dynamic, object> IQuestion.DefaultValueGetter { get; set; }

        Func<string, object> IQuestion.Transformer { get; set; }

        Func<string, dynamic, bool> IQuestion.RawValueValidator { get; set; }

        Func<object, dynamic, bool> IQuestion.Validator { get; set; }

        internal Question(string name, FunctionOrValue<string> message)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Question name must be specified.", nameof(name));
            if (message == null)
                throw new ArgumentNullException("Question message must be specified.", nameof(message));
            Name = name;
            _message = message;
        }

        public static Question<string> Optional(string name, string message) =>
            new Question<string>(name, message);

        public static Question<TValue> Optional<TValue>(string name, string message) =>
            new Question<TValue>(name, message);

        public static Question<string> Optional(string name, string message, string defaultValue) =>
            new Question<string>(name, message, mustAnswer: false)
                .DefaultValue(ans => defaultValue);

        public static Question<TValue> Optional<TValue>(string name, string message, TValue defaultValue) =>
            new Question<TValue>(name, message, mustAnswer: false)
                .DefaultValue(ans => defaultValue);

        public static Question<string> Mandatory(string name, string message) =>
            new Question<string>(name, message, mustAnswer: true);

        public static Question<TValue> Mandatory<TValue>(string name, string message) =>
            new Question<TValue>(name, message, mustAnswer: true);

        public Question AddBanner(params ColorString[] text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            ((List<ColorString>) Banner).AddRange(text);
            return this;
        }

        public Question When(Func<dynamic, bool> when)
        {
            if (when == null)
                throw new ArgumentNullException(nameof(when));
            if (WhenFn != null)
                throw new ArgumentException($"When condition is already defined for the question {Name}.", nameof(when));
            WhenFn = when;
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
    }

    public sealed class Question<TValue> : Question
    {
        internal Question(string name, string message, bool mustAnswer = false)
            : base(name, message, mustAnswer)
        {
        }

        public Question<TValue> DefaultValue(Func<dynamic, TValue> defaultValueGetter)
        {
            if (defaultValueGetter == null)
                throw new ArgumentNullException(nameof(defaultValueGetter));
            if (DefaultValueGetter != null)
                throw new ArgumentException($"Default value getter is alread defined for the question {Name}.", nameof(defaultValueGetter));
            DefaultValueGetter = ans => defaultValueGetter(ans);
            return this;
        }

        public Question<TValue> Transform(Func<string, TValue> transformer)
        {
            if (transformer == null)
                throw new ArgumentNullException(nameof(transformer));
            if (Transformer != null)
                throw new ArgumentException($"Transformer is already defined for the question {Name}.", nameof(transformer));
            Transformer = value => transformer(value);
            return this;
        }

        public Question<TValue> Validate(Func<TValue, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (Validator != null)
                throw new ArgumentException($"Validator is already defined for the question {Name}.", nameof(validator));
            Validator = (value, answers) => validator((TValue) value);
            return this;
        }

        public Question<TValue> Validate(Func<TValue, dynamic, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (Validator != null)
                throw new ArgumentException($"Validator is already defined for the question {Name}.", nameof(validator));
            Validator = (value, answers) => validator((TValue)value, answers);
            return this;
        }
    }

    public static class TransformExtensions
    {
        public static Question<int> AsInteger(this Question question)
        {

        }
    }
}