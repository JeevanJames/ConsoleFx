// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Help
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandHelpAttribute : HelpAttribute
    {
        public CommandHelpAttribute()
        {
        }

        public CommandHelpAttribute(string description)
            : base(description)
        {
        }

        public CommandHelpAttribute(Type descriptionResourceType, string descriptionResourceName)
            : base(descriptionResourceType, descriptionResourceName)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            return new[] { typeof(Command) };
        }
    }
}
