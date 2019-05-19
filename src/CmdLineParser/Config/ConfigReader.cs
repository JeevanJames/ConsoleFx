#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Collections.Generic;

using ConsoleFx.CmdLine;

namespace ConsoleFx.CmdLine.Parser.Config
{
    public abstract class ConfigReader
    {
        //private Dictionary<string, OverrideBehavior> _optionBehaviors;
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
            argumentBehaviors = new List<OverrideBehavior>(Parser.Command.Arguments.Count);
            optionBehaviors = new Dictionary<string, OverrideBehavior>(Parser.Command.Options.Count);
        }

        protected abstract void IdentifyArgs();

        private IEnumerable<string> Consolidate(IEnumerable<string> specifiedArguments, Options options)
        {
            //TODO: Use options
            if (options != null)
                return specifiedArguments;
            return specifiedArguments;
        }

        protected void AddOption(string name, IEnumerable<string> parameters)
        {
            if (_options is null)
                _options = new List<SpecifiedOption>();
            _options.Add(new SpecifiedOption(name, parameters));
        }

        protected void AddArgument(string value)
        {
            if (_arguments is null)
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
