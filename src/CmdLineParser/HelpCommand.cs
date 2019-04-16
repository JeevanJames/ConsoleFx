using System.Collections.Generic;
using System.Linq;

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.CmdLineParser
{
    public class HelpCommand : Command
    {
        /// <inheritdoc />
        public HelpCommand(params string[] names)
            : base(names)
        {
        }

        /// <inheritdoc />
        public HelpCommand(bool caseSensitive, params string[] names)
            : base(caseSensitive, names)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Option> GetOptions()
        {
            IEnumerable<Option> options = base.GetOptions();
            foreach (Option option in options)
                yield return option;

            yield return new Option("help", "h");
        }

        /// <inheritdoc />
        protected override string PerformCustomValidation(IReadOnlyList<string> arguments, IReadOnlyDictionary<string, object> options)
        {
            if (options["help"] != null)
            {
                if (arguments.Count > 0)
                    return "Cannot specify any other arguments with help";
                if (options.Any(opt => opt.Value != null && opt.Key != "help"))
                    return "Cannot specify any other arguments with help";
            }

            return base.PerformCustomValidation(arguments, options);
        }

        /// <inheritdoc />
        protected override int HandleCommand(IReadOnlyList<string> arguments, IReadOnlyDictionary<string, object> options)
        {
            if (options["help"] != null)
                ConsoleEx.PrintLine(new ColorString().BgBlue("This is the help text"));
            return base.HandleCommand(arguments, options);
        }
    }
}
