#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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
    /// <summary>
    ///     Marks a property in a <see cref="Command"/> class as an <see cref="Argument"/>.
    /// </summary>
    public sealed class ArgumentAttribute : ArgumentOrOptionAttribute, IArgApplicator<Argument>
    {
        /// <summary>
        ///     Gets or sets the order of the argument in the list of arguments.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the argument is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of occurences allowed for the argument. This only
        ///     applies to the last argument. For other arguments, this value can only be one.
        /// </summary>
        public byte MaxOccurences { get; set; } = 1;

        void IArgApplicator<Argument>.Apply(Argument arg, PropertyInfo property)
        {
            ArgumentValueType expectedValueType = arg.GetValueType();
            switch (expectedValueType)
            {
                case ArgumentValueType.Object:
                    if (property.PropertyType != typeof(string))
                        arg.TypeAs(property.PropertyType);
                    break;
                case ArgumentValueType.List:
                    Type itemType = GetCollectionItemType(property);
                    if (itemType is null)
                        throw new ParserException(-1, $"Type for property {property.Name} in command {property.DeclaringType.FullName} should be a generic collection type like IEnumerable<T> or List<T>.");
                    if (itemType != typeof(string))
                        arg.TypeAs(itemType);
                    break;
                default:
                    throw new NotSupportedException($"Unexpected ArgumentValueType value of {expectedValueType}.");
            }
        }
    }
}
