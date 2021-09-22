// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Program
{
    public class HelpAttribute : MetadataAttribute
    {
        public HelpAttribute(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description should be specified.", nameof(description));

            Description = description;
        }

        public string Description { get; }

        public int Order { get; }

        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            yield return new KeyValuePair<string, object>(HelpMetadataKey.Description, Description);
            yield return new KeyValuePair<string, object>(HelpMetadataKey.Order, Order);
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
