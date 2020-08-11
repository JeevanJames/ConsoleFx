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
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     A common implementation of <see cref="INamedObject"/> that can be shared between multiple
    ///     classes that implement the interface.
    ///     <para/>
    ///     Currently, <see cref="Command"/>, <see cref="Option"/> and the HelpBuilder all implement the
    ///     <see cref="INamedObject"/> interface.
    /// </summary>
    internal sealed class NamedObjectImpl : INamedObject
    {
        private readonly Dictionary<string, bool> _names = new Dictionary<string, bool>();

        internal NamedObjectImpl()
        {
            NamePattern = DefaultNamePattern;
        }

        internal NamedObjectImpl(Regex pattern)
        {
            NamePattern = pattern;
        }

        void INamedObject.AddName(string name, bool caseSensitive)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (!NamePattern.IsMatch(name))
                throw new ArgumentException(string.Format(Errors.Arg_Invalid_name, name), nameof(name));
            _names.Add(name, caseSensitive);
        }

        bool INamedObject.HasName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            return _names.Any(kvp =>
            {
                var comparison = kvp.Value
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;
                return name.Equals(kvp.Key, comparison);
            });
        }

        string INamedObject.Name => _names.Count == 0 ? null : _names.First().Key;

        IEnumerable<string> INamedObject.AlternateNames => _names.Skip(count: 1).Select(kvp => kvp.Key);

        IEnumerable<string> INamedObject.AllNames => _names.Select(kvp => kvp.Key);

        private Regex NamePattern { get; }

        private static readonly Regex DefaultNamePattern = new Regex(@"^\w[\w_-]*$");
    }
}
