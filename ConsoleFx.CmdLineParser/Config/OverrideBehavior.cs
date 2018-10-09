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

namespace ConsoleFx.CmdLineParser.Config
{
    public enum OverrideBehavior
    {
        /// <summary>
        ///     Merge unique values from config and specified. In case of conflict, ignore the config values.
        /// </summary>
        MergePreferSpecified,

        /// <summary>
        ///     Merge unique values from config and specified. In case of conflict, ignore the specified values.
        /// </summary>
        MergePreferConfig,

        /// <summary>
        ///     If available, use only specified values and ignore config values.
        /// </summary>
        AlwaysSpecified,

        /// <summary>
        ///     If available, use only config values and ignore specified values.
        /// </summary>
        AlwaysConfig
    }
}