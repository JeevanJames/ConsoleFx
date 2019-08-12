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

using System.Globalization;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents a collection of options. Note: This is not a keyed collection because the key
    ///     can be one of many names.
    /// </summary>
    public sealed class Options : Args<Option>
    {
        protected override string GetDuplicateErrorMessage(string name) =>
            string.Format(CultureInfo.CurrentCulture, Messages.OptionAlreadyExists, name);
    }
}
