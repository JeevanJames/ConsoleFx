// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
