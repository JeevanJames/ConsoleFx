using System;
using System.Collections.Generic;
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

        public static IQuestion Separator() => new StaticText((ColorString)new string('=', 80));

        string IQuestion.Name => throw new NotImplementedException();

        FunctionOrValue<string> IQuestion.Message => throw new NotImplementedException();

        FunctionOrValue<bool> IQuestion.MustAnswer => throw new NotImplementedException();

        AnswersFunc<bool> IQuestion.CanAsk => throw new NotImplementedException();

        FunctionOrValue<ColorString> IQuestion.StaticText => _staticText;

        AnswersFunc<object> IQuestion.DefaultValueGetter => throw new NotImplementedException();

        Func<string, object> IQuestion.Transformer => throw new NotImplementedException();

        Validator<string> IQuestion.RawValueValidator => throw new NotImplementedException();

        Validator<object> IQuestion.Validator => throw new NotImplementedException();
    }
}