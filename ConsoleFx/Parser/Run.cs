using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.Parser
{
    internal sealed class Run
    {
        /// <summary>
        ///     All specified commands.
        ///     Note: We use a <see cref="List{T}" /> instead of the Commands collection here, because we want to avoid the
        ///     duplicate checks, as commands at different levels can have the same name.
        /// </summary>
        internal List<Command> Commands = new List<Command>();

        /// <summary>
        ///     All allowed arguments.
        /// </summary>
        internal Arguments Arguments { get; } = new Arguments();

        /// <summary>
        ///     All allowed options and details of which are specified.
        /// </summary>
        internal List<OptionRun> Options { get; } = new List<OptionRun>();

        internal List<string> Tokens { get; set; }
    }

    public sealed class OptionRun
    {
        internal OptionRun(Option option)
        {
            Option = option;
        }

        internal Option Option { get; }
        internal int Occurences { get; set; }

        //TODO: Optimize initial capacity of this list based on the min and max parameters of the option.
        internal List<string> Parameters { get; } = new List<string>();

        internal void Clear()
        {
            Occurences = 0;
            Parameters.Clear();
        }
    }
}
