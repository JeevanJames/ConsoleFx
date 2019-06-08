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

namespace ConsoleFx.CmdLine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataAttribute : Attribute
    {
        public abstract IEnumerable<KeyValuePair<string, string>> GetMetadata();
    }

    public sealed class HelpAttribute : MetadataAttribute
    {
        public HelpAttribute(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description should be specified.", nameof(description));

            Description = description;
        }

        public HelpAttribute(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name should be specified.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description should be specified.", nameof(description));

            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public int Order { get; }

        public override IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            if (Name != null)
                yield return new KeyValuePair<string, string>("Name", Name);

            yield return new KeyValuePair<string, string>("Description", Description);

            yield return new KeyValuePair<string, string>("Order", Order.ToString());
        }
    }
}
