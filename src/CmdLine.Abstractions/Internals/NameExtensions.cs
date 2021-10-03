// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Internals
{
    internal static class NameExtensions
    {
        internal static IReadOnlyList<string> ConstructNames(this string name, string[] additionalNames)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length == 0)
                throw new ArgumentException("The name cannot be empty or whitespaced.", nameof(name));

            if (additionalNames is null)
                throw new ArgumentNullException(nameof(additionalNames));

            for (int i = 0; i < additionalNames.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(additionalNames[i]))
                {
                    throw new ArgumentException(
                        $"Additional names for arg '{name}' has an null, empty or whitespaced value at index {i}.",
                        nameof(additionalNames));
                }
            }

            string[] names = new string[additionalNames.Length + 1];
            names[0] = name;
            if (additionalNames.Length > 0)
                additionalNames.CopyTo(names, index: 1);

            return names;
        }
    }
}
