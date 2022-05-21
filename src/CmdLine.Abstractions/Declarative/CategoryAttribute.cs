// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    public sealed class CategoryAttribute : MetadataAttribute
    {
        public CategoryAttribute()
        {
        }

        public CategoryAttribute(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify valid category name.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Specify valid category description.", nameof(description));

            Name = name;
            Description = description;
        }

        public CategoryAttribute(Type nameResourceType, string nameResourceName, Type descriptionResourceType,
            string descriptionResourceName)
        {
            NameResourceType = nameResourceType;
            NameResourceName = nameResourceName;
            DescriptionResourceType = descriptionResourceType;
            DescriptionResourceName = descriptionResourceName;
        }

        public string Name { get; set; }

        public Type NameResourceType { get; set; }

        public string NameResourceName { get; set; }

        public string Description { get; set; }

        public Type DescriptionResourceType { get; set; }

        public string DescriptionResourceName { get; set; }

        public override IEnumerable<ArgMetadata> GetMetadata()
        {
            yield return new ArgMetadata(HelpMetadataKey.CategoryName,
                ResolveResourceString(Name, NameResourceType, NameResourceName, required: true));
            yield return new ArgMetadata(HelpMetadataKey.CategoryDescription,
                ResolveResourceString(Description, DescriptionResourceType, DescriptionResourceName, required: true));
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            return CommonApplicableArgTypes.All;
        }
    }
}
