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

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Specifies whether an option is required or optional.
    /// </summary>
    public enum OptionRequirement
    {
        /// <summary>
        ///     The option is optional (this is the default). Sets the option's MinOccurences property
        ///     to 0 (zero) and MaxOccurences property to 1 (one). However, the MaxOccurence value can
        ///     be increased, and as long as the MinOccurence value is zero, it will be considered optional.
        /// </summary>
        Optional,

        /// <summary>
        ///     The option is optional. Sets the option's MinOccurences property to 0 and MaxOccurences
        ///     property to int.MaxValue to indicate unlimited number of occurences.
        /// </summary>
        OptionalUnlimited,

        /// <summary>
        ///     The option is required. Sets the option's MinOccurences and MaxOccurences properties to 1 (one).
        /// </summary>
        Required,

        /// <summary>
        ///     The option is required. Sets the option's MinOccurences property to 1 and MaxOccurences
        ///     property to int.MaxValue to indicate unlimited number of occurences.
        /// </summary>
        RequiredUnlimited,
    }
}
