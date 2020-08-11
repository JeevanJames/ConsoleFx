#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    public sealed class FlagAttribute : ArgumentOrOptionAttribute, IArgApplicator<Option>
    {
        public FlagAttribute(params string[] names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            Names = names;
        }

        public string[] Names { get; }

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo property)
        {
            if (property.PropertyType != typeof(bool))
                throw new ParserException(-1, $"Type for property {property.Name} in command {property.DeclaringType.FullName} should be an boolean.");
            arg.UsedAsFlag();
        }
    }
}
