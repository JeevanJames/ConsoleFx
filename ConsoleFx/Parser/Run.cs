using System.Collections.Generic;

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
        internal List<ArgumentRun> Arguments { get; } = new List<ArgumentRun>();

        /// <summary>
        ///     All allowed options and details of which are specified.
        /// </summary>
        internal List<OptionRun> Options { get; } = new List<OptionRun>();

        /// <summary>
        ///     All the specified options and argument tokens after accounting for the commands.
        /// </summary>
        internal List<string> Tokens { get; set; }
    }

    internal sealed class ArgumentRun
    {
        internal ArgumentRun(Argument argument)
        {
            Argument = argument;
        }

        internal Argument Argument { get; }

        internal string Value { get; set; }
    }

    public sealed class OptionRun
    {
        internal OptionRun(Option option, Command command)
        {
            Option = option;
            Command = command;
        }

        internal Option Option { get; }

        internal Command Command { get; }

        internal int Occurences { get; set; }

        //TODO: Optimize initial capacity of this list based on the min and max parameters of the option.
        internal List<string> Parameters { get; } = new List<string>();

        /// <summary>
        ///     <para>The final value of the parameters of the option. The actual type depends on how the option is setup.</para>
        ///     If the option allows parameters, then this can be:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>a <see cref="IList{T}" />, if more than one parameters are allowed, or</description>
        ///         </item>
        ///         <item>
        ///             <description>an object of type T, if only one parameter is allowed.</description>
        ///         </item>
        ///     </list>
        ///     If the option does not allow parameters, then this can be:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 an <see cref="int" /> which is the number of times the option is specified (if it allows more
        ///                 than one occurence), or
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 an <see cref="bool" /> which is true if the option was specified otherwise false (if it allows
        ///                 only one occurence).
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        internal object ResolvedValue { get; set; }

        internal void Clear()
        {
            Occurences = 0;
            Parameters.Clear();
        }
    }
}
