using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class MultiLineQuestion : TextEntryQuestion
    {
        private readonly AskerFn _askerFn;

        public MultiLineQuestion(string name, FunctionOrValue<string> message) : base(name, message)
        {
            _askerFn = (q, ans) =>
            {
                ConsoleEx.PrintLine(q.Message.Resolve(ans));
                return string.Empty;
            };
        }

        internal override AskerFn AskerFn => _askerFn;
    }
}