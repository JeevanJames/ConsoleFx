using System;
using System.Collections.Generic;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    public sealed class Banner : IQuestion
    {
        private readonly FunctionOrValue<IReadOnlyList<ColorString>> _banner;

        public Banner(params ColorString[] banner)
        {
            if (banner == null)
                throw new ArgumentNullException(nameof(banner));
            _banner = banner;
        }

        public Banner(Func<dynamic, IReadOnlyList<ColorString>> bannerGetter)
        {
            if (bannerGetter == null)
                throw new ArgumentNullException(nameof(bannerGetter));
            _banner = bannerGetter;
        }

        string IQuestion.Name => throw new NotImplementedException();

        FunctionOrValue<string> IQuestion.Message => throw new NotImplementedException();

        FunctionOrValue<bool> IQuestion.MustAnswer => throw new NotImplementedException();

        AnswersFunc<bool> IQuestion.CanAsk => throw new NotImplementedException();

        FunctionOrValue<IReadOnlyList<ColorString>> IQuestion.Banner => _banner;

        AnswersFunc<object> IQuestion.DefaultValueGetter => throw new NotImplementedException();

        Func<string, object> IQuestion.Transformer => throw new NotImplementedException();

        Validator<string> IQuestion.RawValueValidator => throw new NotImplementedException();

        Validator<object> IQuestion.Validator => throw new NotImplementedException();
    }
}