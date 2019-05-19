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

namespace ConsoleFx.CmdLine.Program.HelpBuilders
{
    public class DefaultHelpBuilder : HelpBuilder
    {
        public override void DisplayHelp(ConsoleProgram program)
        {
            throw new System.NotImplementedException();
        }

        public UsageType UsageType { get; set; }
    }

    /// <summary>
    ///     Describes how the program usage is to be displayed.
    /// </summary>
    public enum UsageType
    {
        /// <summary>
        ///     The program usage is to be displayed as a summary, showing just the existence of
        ///     subcommands, arguments and options, but not mentioning them in detail.
        /// </summary>
        Summary,

        /// <summary>
        ///     The program usage is to be displayed in detail, with every argument and option
        ///     combinations mentioned.
        /// </summary>
        Detailed,
    }
}
