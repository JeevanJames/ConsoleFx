using System;

namespace ConsoleFx.CmdLine
{
    public sealed class ArgMetadata
    {
        public ArgMetadata(string name, object value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }

        public void Deconstruct(out string name, out object value)
        {
            name = Name;
            value = Value;
        }
    }
}
