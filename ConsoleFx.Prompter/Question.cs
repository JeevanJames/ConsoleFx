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

        public static ListQuestion List(string name, FunctionOrValue<string> message, IEnumerable<string> choices) =>
            new ListQuestion(name, message, choices);
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

        public static StaticText Separator(char separator = '=') => Text(new string(separator, Console.WindowWidth));
    }

    public abstract class TextEntryQuestion : Question
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

    public sealed class ConfirmQuestion : Question
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

    public sealed class ListQuestion : Question
    {
        private readonly IReadOnlyList<string> _choices;
        private readonly AskerFn _askerFn;

        public ListQuestion(string name, FunctionOrValue<string> message, IEnumerable<string> choices) : base(name, message)
        {
            if (choices == null)
                throw new ArgumentNullException(nameof(choices));
            _choices = choices.ToList();

            _askerFn = (q, ans) =>
            {
                var lq = (ListQuestion) q;
                ConsoleEx.PrintLine(q.Message.Resolve(ans));
                return ConsoleEx.SelectSingle(lq._choices);
            };
        }

        internal override AskerFn AskerFn => _askerFn;
    }

    // string[]
    public sealed class CheckboxQuestion
    {
    }
}