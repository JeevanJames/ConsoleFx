#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

namespace ConsoleFx.Prompter
{
    public readonly struct FunctionOrValue<TValue>
    {
        internal TValue Value { get; }

        internal Func<dynamic, TValue> Function { get; }

        internal FunctionOrValue(TValue value)
        {
            Value = value;
            Function = null;
        }

        internal FunctionOrValue(Func<dynamic, TValue> function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
            Function = function;
            Value = default;
        }

        internal TValue Resolve(dynamic answers = null) =>
            Function != null ? (TValue)Function(answers) : Value;

        public static implicit operator FunctionOrValue<TValue>(TValue value) => new FunctionOrValue<TValue>(value);

        public static implicit operator FunctionOrValue<TValue>(Func<dynamic, TValue> function) => new FunctionOrValue<TValue>(function);
    }
}