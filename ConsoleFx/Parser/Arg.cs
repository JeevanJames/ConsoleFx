using System;
using System.Collections.Generic;

namespace ConsoleFx.Parser
{
    /// <summary>
    ///     Base class for <see cref="T:ConsoleFx.Parser.Option" /> and <see cref="T:ConsoleFx.Parser.Argument" />.
    /// </summary>
    public abstract partial class Arg
    {
        /// <summary>
        ///     Optional metadata that can be used by ancillary frameworks, such as the usage builders.
        ///     This is simply a key-value structure.
        /// </summary>
        public CustomMetadata Metadata { get; } = new CustomMetadata();
    }

    public abstract partial class Arg
    {
        public sealed class CustomMetadata
        {
            private Dictionary<string, object> _metadata;

            public string this[string name]
            {
                get { return Get<string>(name); }
                set { Set(name, value); }
            }

            public T Get<T>(string name)
            {
                if (_metadata == null)
                    return default(T);
                object result;
                return _metadata.TryGetValue(name, out result) ? (T)result : default(T);
            }

            public void Set<T>(string name, T value)
            {
                if (_metadata == null)
                    _metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                if (_metadata.ContainsKey(name))
                    _metadata[name] = value;
                else
                    _metadata.Add(name, value);
            }
        }
    }
}