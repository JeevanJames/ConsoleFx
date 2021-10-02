// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace ConsoleFx.CmdLine
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FlagAttribute : Attribute, IArgApplicator<Option>
    {
        public FlagAttribute(params string[] names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            Names = names;
        }

        public string[] Names { get; }

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(bool))
                throw new ParserException(-1, $"Type for property {propertyInfo.Name} in command {propertyInfo.DeclaringType} should be an boolean.");
            arg.UsedAsFlag();
        }
    }
}
