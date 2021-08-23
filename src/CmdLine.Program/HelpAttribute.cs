﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Program
{
    public sealed class HelpAttribute : MetadataAttribute
    {
        public HelpAttribute(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description should be specified.", nameof(description));

            Description = description;
        }

        public HelpAttribute(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name should be specified.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description should be specified.", nameof(description));

            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public int Order { get; }

        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            if (Name != null)
                yield return new KeyValuePair<string, object>(HelpMetadataKey.Name, Name);

            yield return new KeyValuePair<string, object>(HelpMetadataKey.Description, Description);

            yield return new KeyValuePair<string, object>(HelpMetadataKey.Order, Order);
        }
    }
}
