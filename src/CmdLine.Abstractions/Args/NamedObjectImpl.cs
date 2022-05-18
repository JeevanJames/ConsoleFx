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
        private readonly Dictionary<string, bool> _names = new(StringComparer.Ordinal);
        private readonly Regex _namePattern;
        private readonly string _emptyOrWhitespaceNameError;
        private readonly string _invalidNameError;

        internal NamedObjectImpl(string emptyOrWhitespaceNameError, string invalidNameError)
            : this(DefaultNamePattern, emptyOrWhitespaceNameError, invalidNameError)
        {
        }

        internal NamedObjectImpl(Regex pattern, string emptyOrWhitespaceNameError, string invalidNameError)
        {
            _namePattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            _emptyOrWhitespaceNameError = emptyOrWhitespaceNameError;
            _invalidNameError = invalidNameError;
        }

        void INamedObject.AddName(string name, bool caseSensitive)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length == 0)
                throw new ArgumentException(_emptyOrWhitespaceNameError, nameof(name));
            if (!_namePattern.IsMatch(name))
                throw new ArgumentException(string.Format(_invalidNameError, name), nameof(name));

            //TODO: Need to check if the name is already present in the dictionary, but should do it
            //while respecting the caseSensitive param.

            _names.Add(name, caseSensitive);
        }

        bool INamedObject.HasName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return _names.Any(kvp => name.Equals(kvp.Key, kvp.Value
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase));
        }

        string INamedObject.Name => _names.Count == 0 ? null : _names.First().Key;

        IEnumerable<string> INamedObject.AlternateNames => _names.Skip(count: 1).Select(kvp => kvp.Key);

        IEnumerable<string> INamedObject.AllNames => _names.Select(kvp => kvp.Key);

        private static readonly Regex DefaultNamePattern = new(@"^\w[\w_-]*$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));
    }
}
