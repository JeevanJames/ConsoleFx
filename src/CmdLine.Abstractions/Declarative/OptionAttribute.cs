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
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    public sealed class OptionAttribute : ArgumentOrOptionAttribute, IArgApplicator<Option>
    {
        public OptionAttribute(params string[] names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            Names = names;
        }

        public string[] Names { get; }

        public CommonOptionUsage Usage { get; set; }

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo property)
        {
            switch (Usage)
            {
                case CommonOptionUsage.SingleParameter:
                    arg.UsedAsSingleParameter();
                    break;
                case CommonOptionUsage.SingleOccurrenceUnlimitedParameters:
                    arg.UsedAsSingleOccurrenceAndUnlimitedParameters();
                    break;
                case CommonOptionUsage.UnlimitedOccurrencesSingleParameter:
                    arg.UsedAsUnlimitedOccurrencesAndSingleParameter();
                    break;
                case CommonOptionUsage.UnlimitedOccurrencesAndParameters:
                    arg.UsedAsUnlimitedOccurrencesAndParameters();
                    break;
            }

            OptionValueType expectedValueType = arg.GetValueType();
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
    }
}
