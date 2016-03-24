using System;
using System.Collections.Generic;

namespace ConsoleFx.Parser
{
    public abstract class BaseParseResult
    {
        private Dictionary<string, object> _options = new Dictionary<string, object>();
        private ParseCommandResult _command;

        public T OptionAs<T>(string name, T @default = default(T))
        {
            object value;
            return _options.TryGetValue(name, out value) ? (T) value : @default;
        }

        public IReadOnlyList<T> OptionAsListOf<T>(string name)
        {
            object value;
            return _options.TryGetValue(name, out value) ? (List<T>) value : null;
        }

        public string Option(string name)
        {
            object value;
            return _options.TryGetValue(name, out value) ? (string) value : null;
        }

        public IReadOnlyList<string> OptionsAsList(string name)
        {
            object value;
            return _options.TryGetValue(name, out value) ? (List<string>)value : null;
        }
    }

    public sealed class ParseResult : BaseParseResult
    {
        private IReadOnlyList<string> _arguments = new List<string>();
    }

    public class ParseCommandResult : BaseParseResult
    {
        //private string _name;
    }
}