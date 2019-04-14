#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

namespace ConsoleFx.CmdLineParser
{
    /// <summary>
    ///     Specifies how the command-line args are expected to be grouped.
    /// </summary>
    public enum ArgGrouping
    {
        /// <summary>
        ///     Command line parameters grouping does not matter. Options and arguments can be mixed together.
        ///     <para/>
        ///     This is the default grouping.
        /// </summary>
        DoesNotMatter,

        /// <summary>
        ///     Options must be specified before arguments in the command line.
        /// </summary>
        OptionsBeforeArguments,

        /// <summary>
        ///     Options must be specified after arguments in the command line.
        /// </summary>
        OptionsAfterArguments
    }
}
