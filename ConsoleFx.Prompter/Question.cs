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

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    public abstract partial class Question
    {
        protected Question(string name, FunctionOrValue<string> message)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify a name for the question.", nameof(name));
            Name = name;
            Message = message;
        }

        public string Name { get; }

        internal FunctionOrValue<string> Message { get; }

        internal abstract AskerFn AskerFn { get; }

        internal AnswersFunc<bool> CanAskFn { get; set; }

        internal Validator<string> RawValueValidator { get; set; }

        internal Validator<object> Validator { get; set; }

        public Question When(AnswersFunc<bool> canAskFn)
        {
            CanAskFn = canAskFn;
            return this;
        }

        internal bool CanAsk(dynamic answers)
        {
            return CanAskFn != null ? (bool)CanAskFn(answers) : true;
        }
    }

    public abstract partial class Question
    {
        public static InputQuestion Input(string name, FunctionOrValue<string> message) =>
            new InputQuestion(name, message);

        public static PasswordQuestion Password(string name, FunctionOrValue<string> message) =>
            new PasswordQuestion(name, message);

        public static ConfirmQuestion Confirm(string name, FunctionOrValue<string> message, bool @default = false) =>
            new ConfirmQuestion(name, message, @default);
    }

    public sealed class StaticText : Question
    {
        private readonly AskerFn _askerFn;

        internal StaticText(FunctionOrValue<string> message) : base(Guid.NewGuid().ToString("N"), message)
        {
            _askerFn = (q, ans) =>
            {
                ColorString staticText = q.Message.Resolve(ans);
                if (staticText != null)
                    ConsoleEx.PrintLine(staticText);
                return null;
            };
        }

        internal override AskerFn AskerFn => _askerFn;

        public static StaticText Text(FunctionOrValue<string> text) => new StaticText(text);

        public static StaticText BlankLine() => Text(string.Empty);

        public static StaticText Separator() => Text(new string('=', Console.WindowWidth));
    }

    public abstract class Question<T> : Question
    {
        internal Question(string name, FunctionOrValue<string> message) : base(name, message)
        {
        }
    }

    public abstract class TextEntryQuestion : Question<string>
    {
        protected TextEntryQuestion(string name, FunctionOrValue<string> message) : base(name, message)
        {
        }

        public TextEntryQuestion Validate(Func<string, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            RawValueValidator = (str, _) => validator(str);
            return this;
        }

        public TextEntryQuestion Validate(Func<string, dynamic, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            RawValueValidator = (str, ans) => validator(str, ans);
            return this;
        }

        public TextEntryQuestion Required(bool allowWhitespaceOnly = false)
        {
            IsRequired = true;
            AllowWhitespaceOnly = allowWhitespaceOnly;
            return this;
        }

        internal bool IsRequired { get; set; }

        internal bool AllowWhitespaceOnly { get; set; }
    }

    public sealed class InputQuestion : TextEntryQuestion
    {
        private readonly AskerFn _askerFn;

        internal InputQuestion(string name, FunctionOrValue<string> message) : base(name, message)
        {
            _askerFn = (q, ans) =>
            {
                Func<string, bool> validator = str =>
                {
                    bool valid = q.RawValueValidator != null ? q.RawValueValidator(str, ans).Valid : true;
                    var teq = (TextEntryQuestion)q;
                    if (valid && teq.IsRequired)
                    {
                        return teq.AllowWhitespaceOnly
                            ? !string.IsNullOrEmpty(str)
                            : !string.IsNullOrWhiteSpace(str);
                    }
                    return valid;
                };

                return ConsoleEx.Prompt(new ColorString().Cyan(q.Message.Resolve(ans)), validator);
            };
        }

        internal override AskerFn AskerFn => _askerFn;
    }

    public sealed class PasswordQuestion : TextEntryQuestion
    {
        private readonly AskerFn _askerFn;
        private bool _hideCursor;
        private bool _hideMask;

        internal PasswordQuestion(string name, FunctionOrValue<string> message) : base(name, message)
        {
            _askerFn = (q, ans) =>
            {
                var pq = (PasswordQuestion) q;
                return ConsoleEx.ReadSecret(new ColorString().Cyan(q.Message.Resolve(ans)),
                    hideCursor: pq._hideCursor, hideMask: pq._hideMask);
            };
        }

        public PasswordQuestion HideCursor()
        {
            _hideCursor = true;
            return this;
        }

        public PasswordQuestion HideMask()
        {
            _hideMask = true;
            return this;
        }

        internal override AskerFn AskerFn => _askerFn;
    }

    public sealed class ConfirmQuestion : Question<bool>
    {
        private readonly AskerFn _askerFn;
        private readonly bool _default;

        internal ConfirmQuestion(string name, FunctionOrValue<string> message, bool @default) : base(name, message)
        {
            _askerFn = (q, ans) =>
            {
                ConsoleEx.Print(q.Message.Resolve(ans));

                var cq = (ConfirmQuestion) q;
                ConsoleEx.Print($" ({(cq._default ? 'Y' : 'y')}/{(cq._default ? 'n' : 'N')}) ");

                ConsoleKey keyPressed = ConsoleEx.WaitForKeys(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.Enter);
                bool result = keyPressed == ConsoleKey.Enter ? cq._default : (keyPressed == ConsoleKey.Y);

                ConsoleEx.PrintLine(result ? "Y" : "N");

                return result;
            };
            _default = @default;
        }

        internal override AskerFn AskerFn => _askerFn;
    }

    // string
    public sealed class ListQuestion
    {
    }

    // string[]
    public sealed class CheckboxQuestion
    {
    }

    //public sealed partial class Question : IQuestion
    //{
    //    private readonly FunctionOrValue<string> _message;
    //    private FunctionOrValue<bool> _optional;
    //    private AnswersFunc<bool> _canAsk;
    //    private Validator<string> _rawValueValidator;
    //    private Func<string, object> _transformer;
    //    private readonly AskerFn _asker;

    //    public string Name { get; }

    //    FunctionOrValue<string> IQuestion.MessageFn => _message;

    //    FunctionOrValue<bool> IQuestion.OptionalFn => _optional;

    //    AnswersFunc<bool> IQuestion.CanAskFn => _canAsk;

    //    FunctionOrValue<ColorString> IQuestion.StaticTextFn => throw new NotImplementedException();

    //    AnswersFunc<object> IQuestion.DefaultValueFn => null;

    //    Validator<string> IQuestion.RawValueValidatorFn => _rawValueValidator;

    //    Func<string, object> IQuestion.TransformerFn => _transformer;

    //    Validator<object> IQuestion.ValidatorFn => null;

    //    AskerFn IQuestion.AskerFn => _asker;

    //    internal Question(string name, FunctionOrValue<string> message, AskerFn asker)
    //    {
    //        if (string.IsNullOrWhiteSpace(name))
    //            throw new ArgumentException("Question name must be specified.", nameof(name));
    //        Name = name;
    //        _message = message;
    //        _asker = asker;
    //    }

    //    public Question Optional()
    //    {
    //        _optional = true;
    //        return this;
    //    }

    //    public Question Optional(FunctionOrValue<bool> optional)
    //    {
    //        _optional = optional;
    //        return this;
    //    }

    //    public Question When(AnswersFunc<bool> when)
    //    {
    //        if (when == null)
    //            throw new ArgumentNullException(nameof(when));
    //        if (_canAsk != null)
    //            throw new ArgumentException($"When condition is already defined for the question {Name}.", nameof(when));
    //        _canAsk = when;
    //        return this;
    //    }

    //    public Question Validate(Validator<string> validator)
    //    {
    //        if (validator == null)
    //            throw new ArgumentNullException(nameof(validator));
    //        if (_rawValueValidator != null)
    //            throw new ArgumentException($"Raw value validator is already defined for the question {Name}.", nameof(validator));
    //        _rawValueValidator = validator;
    //        return this;
    //    }

    //    public Question<TValue> Transform<TValue>(Func<string, TValue> transformer)
    //    {
    //        if (transformer == null)
    //            throw new ArgumentNullException(nameof(transformer));
    //        _transformer = str => transformer(str);
    //        return new Question<TValue>(this);
    //    }
    //}

    //public sealed partial class Question
    //{
    //    public static Question Input(string name, FunctionOrValue<string> message)
    //    {
    //        AskerFn asker = (q, ans) => ConsoleEx.Prompt(new ColorString().Cyan(q.MessageFn.Resolve(ans)));
    //        return new Question(name, message, asker);
    //    }

    //    public static Question Password(string name, FunctionOrValue<string> message)
    //    {
    //        AskerFn asker = (q, ans) => ConsoleEx.ReadSecret(new ColorString().Cyan(q.MessageFn.Resolve(ans)));
    //        return new Question(name, message, asker);
    //    }

    //    public static Question<bool> Confirm(string name, FunctionOrValue<string> message)
    //    {
    //        AskerFn asker = (q, ans) =>
    //        {
    //            ConsoleEx.Print(q.MessageFn.Resolve(ans) + "(y/n)");
    //            ConsoleKey pressed = ConsoleEx.WaitForKeys(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.Enter);
    //            ConsoleEx.PrintBlank();
    //            return pressed.ToString();
    //        };
    //        return new Question(name, message, asker)
    //            .Transform(str => str == ConsoleKey.Enter.ToString() || str == ConsoleKey.Y.ToString());
    //    }
    //}

    //public sealed class Question<TValue> : IQuestion
    //{
    //    private readonly FunctionOrValue<string> _message;
    //    private FunctionOrValue<bool> _mustAnswer;
    //    private AnswersFunc<bool> _canAsk;
    //    private AnswersFunc<object> _defaultValueGetter;
    //    private Validator<string> _rawValueValidator;
    //    private Func<string, object> _transformer;
    //    private Validator<object> _validator;
    //    private AskerFn _asker;

    //    public string Name { get; }

    //    FunctionOrValue<string> IQuestion.MessageFn => _message;

    //    FunctionOrValue<bool> IQuestion.OptionalFn => _mustAnswer;

    //    AnswersFunc<bool> IQuestion.CanAskFn => _canAsk;

    //    FunctionOrValue<ColorString> IQuestion.StaticTextFn => throw new NotImplementedException();

    //    AnswersFunc<object> IQuestion.DefaultValueFn => _defaultValueGetter;

    //    Validator<string> IQuestion.RawValueValidatorFn => _rawValueValidator;

    //    Func<string, object> IQuestion.TransformerFn => _transformer;

    //    Validator<object> IQuestion.ValidatorFn => _validator;

    //    AskerFn IQuestion.AskerFn => _asker;

    //    internal Question(IQuestion question)
    //    {
    //        Name = question.Name;
    //        _message = question.MessageFn;
    //        _mustAnswer = question.OptionalFn;
    //        _canAsk = question.CanAskFn;
    //        _defaultValueGetter = question.DefaultValueFn;
    //        _rawValueValidator = question.RawValueValidatorFn;
    //        _transformer = question.TransformerFn;
    //        _validator = question.ValidatorFn;
    //        _asker = question.AskerFn;
    //    }

    //    public Question<TValue> DefaultValue(AnswersFunc<TValue> defaultValueGetter)
    //    {
    //        if (defaultValueGetter == null)
    //            throw new ArgumentNullException(nameof(defaultValueGetter));
    //        _defaultValueGetter = ans => defaultValueGetter(ans);
    //        return this;
    //    }

    //    public Question<TValue> Validate(Func<TValue, ValidationResult> validator)
    //    {
    //        if (validator == null)
    //            throw new ArgumentNullException(nameof(validator));
    //        _validator = (value, answers) => validator((TValue) value);
    //        return this;
    //    }

    //    public Question<TValue> Validate(Validator<TValue> validator)
    //    {
    //        if (validator == null)
    //            throw new ArgumentNullException(nameof(validator));
    //        _validator = (value, answers) => validator((TValue)value, answers);
    //        return this;
    //    }
    //}

    //public static class TransformExtensions
    //{
    //    public static Question<int> AsInteger(this Question question) =>
    //        question.Transform(str => int.Parse(str));
    //}
}