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
    ///     Specifies whether the parameters for an option are required, optional or not allowed.
    /// </summary>
    public enum OptionParameterRequirement
    {
        /// <summary>
        ///     Parameters are not allowed for the option.
        /// </summary>
        NotAllowed,

        /// <summary>
        ///     The option allows 0 to 1 parameters.
        /// </summary>
        Optional,

        /// <summary>
        ///     The option allows 0 to unlimited parameters.
        /// </summary>
        //TODO: Should we remove based on the comments on the ValidationExtensions.ParametersOptional method?
        OptionalUnlimited,

        /// <summary>
        ///     The option allows 1 parameter.
        /// </summary>
        Required,

        /// <summary>
        ///     The option allows 1 to unlimited parameters.
        /// </summary>
        RequiredUnlimited
    }
}