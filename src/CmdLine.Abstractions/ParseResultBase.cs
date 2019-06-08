using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLine
{
    [DebuggerDisplay("Command: {Command.Name}")]
    public abstract class ParseResultBase
    {
        public Command Command { get; protected set; }

        /// <summary>
        ///     Gets or sets the list of specified command line arguments.
        /// </summary>
        public IReadOnlyList<object> Arguments { get; protected set; }

        /// <summary>
        ///     Gets or sets the list of specified command line options.
        /// </summary>
        public IReadOnlyDictionary<string, object> Options { get; protected set; }

        public abstract bool TryGetArgument<T>(int index, out T value, T @default = default);

        public abstract bool TryGetArgument<T>(string name, out T value, T @default = default);

        public abstract bool TryGetOption<T>(string name, out T value, T @default = default);

        public abstract bool TryGetOptions<T>(string name, out IReadOnlyList<T> values);
    }
}
