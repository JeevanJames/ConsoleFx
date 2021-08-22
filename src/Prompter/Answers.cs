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
using System.Collections.Generic;
using System.Dynamic;

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Represents a dictionary of the answered values.
    /// </summary>
    public sealed class Answers : DynamicObject
    {
        private readonly Dictionary<string, object> _answers;

        internal Answers(int capacity)
        {
            _answers = new Dictionary<string, object>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_answers.TryGetValue(binder.Name, out result))
                result = null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!_answers.ContainsKey(binder.Name))
                return false;
            _answers[binder.Name] = value;
            return true;
        }

        internal void Add(string name, object value)
        {
            _answers.Add(name, value);
        }
    }
}
