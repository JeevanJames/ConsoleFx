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

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Encapsulates a method that performs custom validation once a <see cref="Command"/> instance
    ///     has been initialized.
    /// </summary>
    /// <param name="arguments">The arguments passed to the command.</param>
    /// <param name="options">The options passed to the command.</param>
    /// <returns>A validation error message, if the validation fails; otherwise <c>null</c>.</returns>
    public delegate string CommandCustomValidator(IReadOnlyList<object> arguments,
        IReadOnlyDictionary<string, object> options);
}
