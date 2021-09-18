// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Program
{
    public sealed class CategoryAttribute : MetadataAttribute
    {
        public CategoryAttribute(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify valid category name.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Specify valid category description.", nameof(description));

            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            yield return new KeyValuePair<string, object>(HelpMetadataKey.CategoryName, Name);
            yield return new KeyValuePair<string, object>(HelpMetadataKey.CategoryDescription, Description);
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            yield return typeof(Command);
            yield return typeof(Option);
            yield return typeof(Argument);
        }
    }
}
