using System.Collections.Generic;

namespace ConsoleFx.Parser.Config
{
    public abstract class ConfigReader
    {
        private Dictionary<string, OverrideBehavior> _optionBehaviors;

        private List<SpecifiedOption> _options;
        private List<string> _arguments = new List<string>();

        internal Parser Parser { get; set; }

        public OverrideBehavior OverrideBehavior { get; set; } = OverrideBehavior.MergePreferSpecified;

        internal IEnumerable<string> Run(IEnumerable<string> specifiedArguments, Options options)
        {
            IdentifyArgs();
            return Consolidate(specifiedArguments, options);
        }

        private void InitializeBehaviors(out List<OverrideBehavior> argumentBehaviors,
            out Dictionary<string, OverrideBehavior> optionBehaviors)
        {
            argumentBehaviors = new List<OverrideBehavior>(Parser.Arguments.Count);
            optionBehaviors = new Dictionary<string, OverrideBehavior>(Parser.Options.Count);
        }

        protected abstract void IdentifyArgs();

        private IEnumerable<string> Consolidate(IEnumerable<string> specifiedArguments, Options options)
        {
            return specifiedArguments;
        }

        protected void AddOption(string name, IEnumerable<string> parameters)
        {
            if (_options == null)
                _options = new List<SpecifiedOption>();
            _options.Add(new SpecifiedOption(name, parameters));
        }

        protected void AddArgument(string value)
        {
            if (_arguments == null)
                _arguments = new List<string>();
            _arguments.Add(value);
        }
    }

    internal sealed class SpecifiedOption
    {
        internal SpecifiedOption(string name, IEnumerable<string> parameters)
        {
            Name = name;
            Parameters = parameters != null ? new List<string>(parameters) : null;
        }

        internal string Name { get; }

        internal List<string> Parameters { get; }
    }
}