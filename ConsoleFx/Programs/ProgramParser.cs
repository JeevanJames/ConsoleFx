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

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;

namespace ConsoleFx.Programs
{
    /// <summary>
    ///     Parser specific to console programs. Adds some commandline-specific behaviors.
    /// </summary>
    /// <typeparam name="TStyle">The parser style to use.</typeparam>
    public abstract class ProgramParser<TStyle> : BaseParser<TStyle>
        where TStyle : ParserStyle, new()
    {
        public bool DisplayUsageOnError { get; set; }

        public Argument AddArgument(bool optional = false) => InternalAddArgument(optional);

        public Option AddOption(string name, string shortName = null, bool caseSensitive = false,
            int order = int.MaxValue) =>
                InternalAddOption(name, shortName, caseSensitive, order);
    }
}