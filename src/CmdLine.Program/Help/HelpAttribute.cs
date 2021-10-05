// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleFx.CmdLine.Help
{
    public abstract class HelpAttribute : MetadataAttribute
    {
        protected HelpAttribute()
        {
        }

        protected HelpAttribute(string description)
        {
            Description = description;
        }

        protected HelpAttribute(Type descriptionResourceType, string descriptionResourceName)
        {
            DescriptionResourceType = descriptionResourceType ?? throw new ArgumentNullException(nameof(descriptionResourceType));
            DescriptionResourceName = descriptionResourceName ?? throw new ArgumentNullException(nameof(descriptionResourceName));
        }

        public string Description { get; set; }

        public string DescriptionResourceName { get; set; }

        public Type DescriptionResourceType { get; set; }

        public int Order { get; set; }

        public override IEnumerable<KeyValuePair<string, object>> GetMetadata()
        {
            yield return new KeyValuePair<string, object>(HelpMetadataKey.Description,
                ResolveString(Description, DescriptionResourceType, DescriptionResourceName, required: true));
            yield return new KeyValuePair<string, object>(HelpMetadataKey.Order, Order);
        }

        protected static string ResolveString(string unlocalizedValue, Type resourceType, string resourceName,
            bool required)
        {
            if (resourceType is not null && !string.IsNullOrWhiteSpace(resourceName))
            {
                PropertyInfo resourceProperty = resourceType.GetTypeInfo().GetDeclaredProperty(resourceName);
                if (resourceProperty is null)
                {
                    throw new ParserException(-1,
                        $"Resource type {resourceType} does not contain a resource named {resourceName}.");
                }

                if (resourceProperty.PropertyType != typeof(string))
                {
                    throw new ParserException(-1,
                        $"Resource {resourceName} on the resource type {resourceType} is not a string.");
                }

                return resourceProperty.GetValue(null, null) as string;
            }

            if (unlocalizedValue is not null)
                return unlocalizedValue;

            if (required)
                throw new ParserException(-1, "Specify either a string value or a resource type/name pair.");

            return null;
        }
    }
}
