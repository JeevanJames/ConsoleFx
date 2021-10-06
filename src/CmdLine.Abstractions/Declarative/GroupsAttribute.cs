// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class GroupsAttribute : Attribute, IArgApplicator<Option>, IArgApplicator<Argument>
    {
        public GroupsAttribute(params int[] groups)
        {
            if (groups is null)
                throw new ArgumentNullException(nameof(groups));
            Groups = groups.Length == 0 ? new[] { 0 } : groups;
        }

        public int[] Groups { get; }

        /// <inheritdoc />
        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo propertyInfo)
        {
            arg.UnderGroups(Groups);
        }

        /// <inheritdoc />
        void IArgApplicator<Argument>.Apply(Argument arg, PropertyInfo propertyInfo)
        {
            arg.UnderGroups(Groups);
        }
    }
}
