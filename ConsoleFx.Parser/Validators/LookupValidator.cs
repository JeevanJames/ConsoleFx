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
using System.Linq;

namespace ConsoleFx.Parser.Validators
{
    public sealed class LookupValidator : SingleMessageValidator
    {
        private readonly List<string> _items;

        public LookupValidator(IEnumerable<string> items)
            : base(Messages.Lookup)
        {
            _items = items != null ? new List<string>(items) : new List<string>();
        }

        public void Add(string item)
        {
            _items.Add(item);
        }

        public void AddRange(IEnumerable<string> items)
        {
            _items.AddRange(items);
        }

        public override void Validate(string parameterValue)
        {
            StringComparison comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            if (!_items.Any(item => parameterValue.Equals(item, comparison)))
                ValidationFailed(parameterValue);
        }

        public bool CaseSensitive { get; set; }
    }
}