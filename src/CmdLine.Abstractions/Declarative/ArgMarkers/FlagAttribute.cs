// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    public sealed class FlagAttribute : ArgumentOrOptionAttribute, IArgApplicator<Option>
    {
        public FlagAttribute(params string[] names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            Names = names;
        }

        public string[] Names { get; }

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo property)
        {
            if (property.PropertyType != typeof(bool))
                throw new ParserException(-1, $"Type for property {property.Name} in command {property.DeclaringType} should be an boolean.");
            arg.UsedAsFlag();
        }
    }
}
