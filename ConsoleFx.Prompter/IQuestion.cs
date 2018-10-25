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
        /// <summary>
        /// The name to use when storing the answer in the answers hash.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// String or function that represents the question to display.
        /// If defined as a function, the first parameter will be the current answers hash.
        /// Defaults to the value of name
        /// </summary>
        FunctionOrValue<string> Message { get; }

        FunctionOrValue<bool> MustAnswer { get; }

        AnswersFunc<bool> CanAsk { get; }

        FunctionOrValue<ColorString> StaticText { get; }

        AnswersFunc<object> DefaultValueGetter { get; }

        Func<string, object> Transformer { get; }

        Validator<string> RawValueValidator { get; }

        Validator<object> Validator { get; }
    }

    public delegate TResult AnswersFunc<TResult>(dynamic answers);

    public delegate ValidationResult Validator<TValue>(TValue value, dynamic answers);
}