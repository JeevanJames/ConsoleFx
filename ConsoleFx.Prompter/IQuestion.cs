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
    public interface IQuestion
    {
        /// <summary>
        /// The name to use when storing the answer in the answers hash.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// String or function that represents the question to display.
        /// If defined as a function, the first parameter will be the current answers hash.
        /// Defaults to the value of name
        /// </summary>
        FunctionOrValue<string> MessageFn { get; }

        FunctionOrValue<bool> OptionalFn { get; }

        AnswersFunc<bool> CanAskFn { get; }

        FunctionOrValue<ColorString> StaticTextFn { get; }

        AnswersFunc<object> DefaultValueFn { get; }

        Func<string, object> TransformerFn { get; }

        Validator<string> RawValueValidatorFn { get; }

        Validator<object> ValidatorFn { get; }

        AskerFn AskerFn { get; }
    }

    public delegate TResult AnswersFunc<TResult>(dynamic answers);

    public delegate ValidationResult Validator<TValue>(TValue value, dynamic answers);

    public delegate string AskerFn(IQuestion question, dynamic answers);
}