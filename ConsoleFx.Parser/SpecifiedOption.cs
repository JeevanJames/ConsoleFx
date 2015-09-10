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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConsoleFx.Parser
{
    /// <summary>
    /// Collection of options specified on the command line and their parameters.
    /// Option name -> SpecifiedOptionParameters
    /// </summary>
    public sealed class SpecifiedOptions : Dictionary<string, SpecifiedOptionParameters>
    {
        public SpecifiedOptions()
        {
        }

        public SpecifiedOptionParameters this[Option option]
        {
            get
            {
                StringComparison comparison = option.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return this
                    .Where(kvp => option.Name.Equals(kvp.Key, comparison) || (!string.IsNullOrEmpty(option.ShortName) && option.ShortName.Equals(kvp.Key, comparison)))
                    .Select(kvp => kvp.Value)
                    .FirstOrDefault();
            }
        }
    }

    public sealed class SpecifiedOptionParameters : Collection<string>
    {
    }
}
