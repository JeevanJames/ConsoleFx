// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.CmdLine.Help
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionHelpAttribute : HelpAttribute
    {
        public OptionHelpAttribute()
        {
        }

        public OptionHelpAttribute(string description)
            : base(description)
        {
        }

        public OptionHelpAttribute(Type descriptionResourceType, string descriptionResourceName)
            : base(descriptionResourceType, descriptionResourceName)
        {
        }

        public string ParameterName { get; set; }

        public Type ParameterNameResourceType { get; set; }

        public string ParameterNameResourceName { get; set; }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            return new[] { typeof(Option) };
        }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            return base.GetMetadata().Concat(new[]
            {
                new KeyValuePair<string, object>(HelpMetadataKey.ParameterName,
                    ResolveString(ParameterName, ParameterNameResourceType, ParameterNameResourceName, required: false)),
            });
        }
    }
}
