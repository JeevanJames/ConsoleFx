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

using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.CmdLine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class CommandAttribute : Attribute
    {
        private Type _parentType;

        public CommandAttribute(string name, Type parentType)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            Names = new[] { name };
            ParentType = parentType;
        }

        public CommandAttribute(params string[] names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            if (names.Length == 0)
                throw new ArgumentException("Specify at least one name for the command.", nameof(names));

            Names = names.ToList();
        }

        public IReadOnlyList<string> Names { get; }

        public Type ParentType
        {
            get => _parentType;
            set
            {
                if (value != null && !typeof(Command).IsAssignableFrom(value))
                    throw new ArgumentException($"ParentType should be type {typeof(Command).FullName} or a derived type.", nameof(value));
                _parentType = value;
            }
        }
    }
}
