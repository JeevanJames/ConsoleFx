﻿#region --- License & Copyright Notice ---
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

namespace ConsoleFx.CmdLine.Program
{
    public sealed class CategoryAttribute : MetadataAttribute
    {
        public CategoryAttribute(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify valid category name.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Specify valid category description.", nameof(description));

            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            yield return new KeyValuePair<string, object>(HelpExtensions.Keys.CategoryName, Name);
            yield return new KeyValuePair<string, object>(HelpExtensions.Keys.CategoryDescription, Description);
        }
    }
}
