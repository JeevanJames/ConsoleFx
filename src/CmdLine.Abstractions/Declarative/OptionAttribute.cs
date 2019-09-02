#region --- License & Copyright Notice ---
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
    public sealed class OptionAttribute : Attribute, IArgApplicator<Option>
    {
        public OptionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

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
}
