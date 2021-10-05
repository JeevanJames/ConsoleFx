// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.CmdLine.Help
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ArgumentHelpAttribute : HelpAttribute
    {
        public ArgumentHelpAttribute()
        {
        }

        public ArgumentHelpAttribute(string name, string description)
            : base(description)
        {
            Name = name;
        }

        public ArgumentHelpAttribute(Type resourceType, string nameResourceName, string descriptionResourceName)
            : base(resourceType, descriptionResourceName)
        {
            NameResourceType = resourceType;
            NameResourceName = nameResourceName;
        }

        public string Name { get; set; }

        public Type NameResourceType { get; set; }

        public string NameResourceName { get; set; }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            return new[] { typeof(Argument) };
        }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            return base.GetMetadata().Concat(new[]
            {
                new KeyValuePair<string, object>(HelpMetadataKey.Name,
                    ResolveString(Name, NameResourceType, NameResourceName, required: true)),
            });
        }
    }
}
