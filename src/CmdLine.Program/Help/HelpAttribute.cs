// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Help
{
    public sealed class HelpAttribute : MetadataAttribute
    {
        public HelpAttribute(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description should be specified.", nameof(description));

            Name = null;
            Description = description;
        }

        public HelpAttribute(string name, string description)
            : this(description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name should be specified.", nameof(name));

            Name = name;
        }

        public string Name { get; }

        public string Description { get; }

        public int Order { get; }

        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            if (Name is not null)
                yield return new KeyValuePair<string, object>(HelpMetadataKey.Name, Name);
            yield return new KeyValuePair<string, object>(HelpMetadataKey.Description, Description);
            yield return new KeyValuePair<string, object>(HelpMetadataKey.Order, Order);
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            if (Name is not null)
                yield return typeof(Argument);
            else
            {
                yield return typeof(Command);
                yield return typeof(Option);
            }
        }
    }
}
