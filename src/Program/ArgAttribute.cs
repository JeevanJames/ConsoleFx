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

namespace ConsoleFx.Program
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ArgAttribute : Attribute
    {
        protected ArgAttribute(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length == 0)
                throw new ArgumentException("Specify the name of the arg.", nameof(name));

            Name = name;
        }

        public string Name { get; }
    }

    public sealed class OptionAttribute : ArgAttribute
    {
        public OptionAttribute(string name)
            : base(name)
        {
        }
    }

    public sealed class ArgumentAttribute : ArgAttribute
    {
        public ArgumentAttribute(string name)
            : base(name)
        {
        }
    }
}
