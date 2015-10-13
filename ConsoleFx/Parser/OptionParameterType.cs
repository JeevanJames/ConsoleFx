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

namespace ConsoleFx.Parser
{
    /// <summary>
    /// Specify whether the parameters of an option are repeating or individual
    /// </summary>
    public enum OptionParameterType
    {
        /// <summary>
        /// The parameters are repeating and have the same meaning
        /// </summary>
        Repeating,

        /// <summary>
        /// Each parameter is independant, has its own meaning and is at a specific position
        /// </summary>
        Individual,
    }
}
