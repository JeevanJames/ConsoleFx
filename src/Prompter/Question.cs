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
        protected Question(string name, FunctionOrValue<string> message)
            : base(message)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify a name for the question.", nameof(name));
            Name = name;
        }

        public string Name { get; }

        internal FunctionOrValue<object> DefaultValue { get; set; }

        internal Validator<object> Validator { get; set; }

        internal Func<object, object> ConverterFn { get; set; }

        internal Validator<object> ConvertedValueValidator { get; set; }

        public new Question When(AnswersFunc<bool> canAskFn)
        {
            CanAskFn = canAskFn;
            return this;
        }

        public Question DefaultsTo(FunctionOrValue<object> defaultValue)
        {
            DefaultValue = defaultValue;
            return this;
        }

        internal object Convert(object value) =>
            ConverterFn != null ? ConverterFn(value) : value;
    }
}
