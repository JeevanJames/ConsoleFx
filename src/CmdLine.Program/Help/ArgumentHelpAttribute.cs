// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.CmdLine.Help
{
    public sealed class ArgumentHelpAttribute : HelpAttribute
    {
        public ArgumentHelpAttribute(string name, string description)
            : base(description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name should be specified.", nameof(name));

            Name = name;
        }

        public string Name { get; }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
#if NETSTANDARD2_0_OR_GREATER
            return base.GetMetadata().Append(new KeyValuePair<string, object>(HelpMetadataKey.Name, Name));
#else
            return new[] { new KeyValuePair<string, object>(HelpMetadataKey.Name, Name) }
                .Concat(base.GetMetadata());
#endif
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            yield return typeof(Argument);
        }
    }
}
