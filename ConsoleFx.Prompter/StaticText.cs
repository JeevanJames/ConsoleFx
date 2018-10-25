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
    public sealed class StaticText : IQuestion
    {
        private readonly FunctionOrValue<ColorString> _staticText;

        internal StaticText(FunctionOrValue<ColorString> staticText)
        {
            _staticText = staticText;
        }

        public static IQuestion Text(FunctionOrValue<ColorString> text) => new StaticText(text);

        public static IQuestion BlankLine() => new StaticText((ColorString)string.Empty);

        public static IQuestion Separator() => new StaticText((ColorString)new string('=', Console.WindowWidth));

        string IQuestion.Name => throw new NotImplementedException();

        FunctionOrValue<string> IQuestion.MessageFn => throw new NotImplementedException();

        FunctionOrValue<bool> IQuestion.OptionalFn => throw new NotImplementedException();

        AnswersFunc<bool> IQuestion.CanAskFn => throw new NotImplementedException();

        FunctionOrValue<ColorString> IQuestion.StaticTextFn => _staticText;

        AnswersFunc<object> IQuestion.DefaultValueFn => throw new NotImplementedException();

        Func<string, object> IQuestion.TransformerFn => throw new NotImplementedException();

        Validator<string> IQuestion.RawValueValidatorFn => throw new NotImplementedException();

        Validator<object> IQuestion.ValidatorFn => throw new NotImplementedException();

        AskerFn IQuestion.AskerFn => throw new NotImplementedException();
    }
}