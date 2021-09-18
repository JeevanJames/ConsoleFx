// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
    ///     Currently, <see cref="Command"/>, <see cref="Option"/> and the HelpBuilder all implement
    ///     the <see cref="INamedObject"/> interface, with the implementation delegated to this class.
    /// </summary>
    internal sealed class NamedObjectImpl : INamedObject
    {
        private readonly Dictionary<string, bool> _names = new();

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
                StringComparison comparison = kvp.Value
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;
                return name.Equals(kvp.Key, comparison);
            });
        }

        string INamedObject.Name => _names.Count == 0 ? null : _names.First().Key;

        IEnumerable<string> INamedObject.AlternateNames => _names.Skip(count: 1).Select(kvp => kvp.Key);

        IEnumerable<string> INamedObject.AllNames => _names.Select(kvp => kvp.Key);

        private Regex NamePattern { get; }

        private static readonly Regex DefaultNamePattern = new(@"^\w[\w_-]*$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));
    }
}
