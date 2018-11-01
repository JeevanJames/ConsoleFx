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
using ConsoleFx.Prompter.Questions;

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

        internal Validator<object> Validator { get; set; }

        internal Func<object, object> ConverterFn { get; set; }

        internal Validator<object> ConvertedValueValidator { get; set; }

        public Question When(AnswersFunc<bool> canAskFn)
        {
            CanAskFn = canAskFn;
            return this;
        }

        internal bool CanAsk(dynamic answers) =>
            CanAskFn != null ? (bool)CanAskFn(answers) : true;

        internal object Convert(object value) =>
            ConverterFn != null ? ConverterFn(value) : value;
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
}