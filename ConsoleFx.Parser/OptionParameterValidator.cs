#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

using ConsoleFx.Parsers.Validators;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConsoleFx.Parser
{
    public sealed class OptionParameterValidators
    {
        public OptionParameterValidators(int parameterIndex)
        {
            ParameterIndex = parameterIndex;
        }

        public int ParameterIndex { get; }

        public ValidatorCollection Validators { get; } = new ValidatorCollection();
    }

    /// <summary>
    /// Collection of all parameter validators of an option. The collection is grouped by parameter index
    /// </summary>
    public sealed class OptionParameterValidatorsCollection : KeyedCollection<int, OptionParameterValidators>
    {
        public void Add(BaseValidator validator)
        {
            Add(-1, validator);
        }

        public void Add(int parameterIndex, BaseValidator validator)
        {
            if (parameterIndex < -1)
                parameterIndex = -1;

            OptionParameterValidators validatorsByIndex = this[parameterIndex];
            if (validatorsByIndex == null)
            {
                validatorsByIndex = new OptionParameterValidators(parameterIndex);
                Add(validatorsByIndex);
            }
            validatorsByIndex.Validators.Add(validator);
        }

        protected override int GetKeyForItem(OptionParameterValidators item)
        {
            return item.ParameterIndex;
        }

        protected override void InsertItem(int index, OptionParameterValidators item)
        {
            //TODO: Ensure that only index is only -1 or specific indices, but not both
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, OptionParameterValidators item)
        {
            //TODO: Ensure that only index is only -1 or specific indices, but not both
            base.SetItem(index, item);
        }

        public new OptionParameterValidators this[int parameterIndex]
        {
            get
            {
                if (Dictionary != null)
                {
                    OptionParameterValidators validators;
                    return Dictionary.TryGetValue(parameterIndex, out validators) ? validators : null;
                }
                return this.FirstOrDefault(vals => vals.ParameterIndex == parameterIndex);
            }
        }
    }
}
