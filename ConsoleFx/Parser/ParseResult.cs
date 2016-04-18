using System.Collections.Generic;

namespace ConsoleFx.Parser
{
    public sealed class ParseResult
    {
        internal ParseResult(Command command, IReadOnlyList<string> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            Command = command;
            Arguments = arguments;
            Options = options;
        }

        public Command Command { get; }

        public IReadOnlyList<string> Arguments { get; }

        public IReadOnlyDictionary<string, object> Options { get; }

        public T OptionAs<T>(string name, T @default = default(T))
        {
            object value;
            return Options.TryGetValue(name, out value) ? (T)value : @default;
        }

        public IReadOnlyList<T> OptionsAsListOf<T>(string name)
        {
            object value;
            return Options.TryGetValue(name, out value) ? (List<T>)value : null;
        }

        public string Option(string name) => OptionAs<string>(name);

        public IReadOnlyList<string> OptionsAsList(string name) => OptionsAsListOf<string>(name);
    }

    //public abstract class BaseParseResult
    //{
    //    public ParseCommandResult Command { get; internal set; }

    //    internal Dictionary<string, object> InternalOptions { get; } = new Dictionary<string, object>();

    //    public IReadOnlyDictionary<string, object> Options => InternalOptions;

    //    public T OptionAs<T>(string name, T @default = default(T))
    //    {
    //        object value;
    //        return Options.TryGetValue(name, out value) ? (T)value : @default;
    //    }

    //    public IReadOnlyList<T> OptionsAsListOf<T>(string name)
    //    {
    //        object value;
    //        return Options.TryGetValue(name, out value) ? (List<T>)value : null;
    //    }

    //    public string Option(string name) => OptionAs<string>(name);

    //    public IReadOnlyList<string> OptionsAsList(string name) => OptionsAsListOf<string>(name);
    //}

    ///// <summary>
    /////     Represents results of parsing a specified set of tokens. This class represents the results for the root command.
    /////     Results for all commands (if any) are stored hierarchially under the <see cref="BaseParseResult.Command" />
    /////     property.
    /////     Options for the root command as well as all subsequent commands are available under the
    /////     <see cref="BaseParseResult.Options" /> property of the respective result classes.
    /////     Since only the arguments of the last command can be considered, these are stored in the root result and not the
    /////     result of the final command, to avoid unnecessary traversal.
    ///// </summary>
    //public sealed class ParseResult : BaseParseResult
    //{
    //    internal ParseResult()
    //    {
    //    }

    //    internal List<string> InternalArguments { get; } = new List<string>();

    //    public IReadOnlyList<string> Arguments => InternalArguments;
    //}

    //public sealed class ParseCommandResult : BaseParseResult
    //{
    //    internal ParseCommandResult(string name)
    //    {
    //        Name = name;
    //    }

    //    public string Name { get; }
    //}
}
