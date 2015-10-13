#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

using ConsoleFx.Parser.Styles;
using System.Collections.Generic;

namespace ConsoleFx.Parser
{
    public class Parser<TStyle> : BaseParser<TStyle>
        where TStyle : ParserStyle, new()
    {
        public Parser()
        {
            Arguments = base.Arguments;
            Options = base.Options;
            Behaviors = base.Behaviors;
        }

        public new Arguments Arguments { get; }
        public new Options Options { get; }
        public new Behaviors Behaviors { get; }

        public new void Parse(IEnumerable<string> tokens)
        {
            base.Parse(tokens);
        }
    }
}
