using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
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
}