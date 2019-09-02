﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ArgAttribute : Attribute
    {
        protected ArgAttribute(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length == 0)
                throw new ArgumentException("Specify the name of the arg.", nameof(name));

            Name = name;
        }

        public string Name { get; }
    }

    public enum CommonUsage
    {
        Flag,
        SingleParameter,
        SingleOccurrenceUnlimitedParameters,
        UnlimitedOccurrencesSingleParameter,
        UnlimitedOccurrencesAndParameters,
    }

    public sealed class OptionAttribute : ArgAttribute, IArgApplicator<Option>
    {
        public OptionAttribute(string name)
            : base(name)
        {
        }

        public CommonUsage Usage { get; set; }

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo property)
        {
            switch (Usage)
            {
                case CommonUsage.Flag:
                    arg.UsedAsFlag();
                    break;
                case CommonUsage.SingleParameter:
                    arg.UsedAsSingleParameter();
                    break;
                case CommonUsage.SingleOccurrenceUnlimitedParameters:
                    arg.UsedAsSingleOccurrenceAndUnlimitedParameters();
                    break;
                case CommonUsage.UnlimitedOccurrencesSingleParameter:
                    arg.UsedAsUnlimitedOccurrencesAndSingleParameter();
                    break;
                case CommonUsage.UnlimitedOccurrencesAndParameters:
                    arg.UsedAsUnlimitedOccurrencesAndParameters();
                    break;
            }

            OptionValueType expectedValueType = arg.GetOptionValueType();
            switch (expectedValueType)
            {
                case OptionValueType.Count:
                    if (property.PropertyType != typeof(int))
                        throw new ParserException(-1, $"Type for property {property.Name} in command {property.DeclaringType.FullName} should be an integer.");
                    break;
                case OptionValueType.Flag:
                    if (property.PropertyType != typeof(bool))
                        throw new ParserException(-1, $"Type for property {property.Name} in command {property.DeclaringType.FullName} should be an boolean.");
                    break;
                case OptionValueType.Object:
                    if (property.PropertyType != typeof(string))
                        arg.TypeAs(property.PropertyType);
                    break;
                case OptionValueType.List:
                    Type itemType = GetCollectionItemType(property);
                    if (itemType is null)
                        throw new ParserException(-1, $"Type for property {property.Name} in command {property.DeclaringType.FullName} should be a generic collection type like IEnumerable<T> or List<T>.");
                    if (itemType != typeof(string))
                        arg.TypeAs(itemType);
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected OptionValueType value of {expectedValueType}.");
            }
        }

        private static Type GetCollectionItemType(PropertyInfo property)
        {
            Type type = property.PropertyType;

            if (!type.IsGenericType)
                return null;

            Type[] genericArgs = type.GetGenericArguments();
            if (genericArgs.Length != 1)
                return null;

            Type collectionType = typeof(IEnumerable<>).MakeGenericType(genericArgs[0]);
            if (!collectionType.IsAssignableFrom(type))
                return null;

            return genericArgs[0];
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class OptionApplicatorAttribute : Attribute, IArgApplicator<Option>
    {
        public abstract void Apply(Option arg, PropertyInfo property);
    }

    public sealed class FlagAttribute : OptionApplicatorAttribute
    {
        public override void Apply(Option arg, PropertyInfo property)
        {
            arg.UsedAsFlag();
        }
    }

    public sealed class SingleParameterAttribute : OptionApplicatorAttribute
    {
        public SingleParameterAttribute(bool optional = true)
        {
            Optional = optional;
        }

        public bool Optional { get; }

        public override void Apply(Option arg, PropertyInfo property)
        {
            arg.UsedAsSingleParameter(Optional);
        }
    }

    public sealed class FormatAsAttribute : OptionApplicatorAttribute
    {
        public FormatAsAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; }

        public override void Apply(Option arg, PropertyInfo property)
        {
            arg.FormatAs(Format);
        }
    }

    public sealed class ArgumentAttribute : ArgAttribute
    {
        public ArgumentAttribute(string name)
            : base(name)
        {
        }

        public bool IsOptional { get; set; }

        public byte MaxOccurences { get; set; } = 1;
    }
}
