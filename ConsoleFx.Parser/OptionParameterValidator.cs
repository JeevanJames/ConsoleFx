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

using ConsoleFx.Parser.Validators;
using System.Collections.Generic;

namespace ConsoleFx.Parser
{
    /// <summary>
    /// Collection of all parameter validators of an option. The collection is grouped by parameter index.
    /// A parameter index of -1 indicates validators for all parameters.
    /// </summary>
    public sealed class OptionParameterValidators
    {
        private readonly Option _option;
        private readonly Dictionary<int, ValidatorCollection> _validators = new Dictionary<int, ValidatorCollection>();

        public OptionParameterValidators(Option option)
        {
            _option = option;
        }

        public void Add(params BaseValidator[] validators)
        {
            Add(-1, validators);
        }

        public void Add(int parameterIndex, params BaseValidator[] validators)
        {
            if (_option.Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
                throw new ParserException(1000, $"Cannot add validators to option {_option.Name} because it does not accept parameters.");
            if (parameterIndex >= 0 && _option.Usage.ParameterType == OptionParameterType.Repeating)
                throw new ParserException(1000, $"Cannot add a specific parameter validator for option {_option.Name} because it is configured to have repeating parameters, where all parameters share the same validators. Use the ValidateWith overload that does not accept a parameter index.");
            if (parameterIndex >= _option.Usage.MaxParameters)
                throw new ParserException(1000, $"Parameter index specified {parameterIndex} is greater than the number of parameters allowed for option {_option.Name}.");

            if (parameterIndex < -1)
                parameterIndex = -1;

            ValidatorCollection validatorList;
            if (!_validators.TryGetValue(parameterIndex, out validatorList))
            {
                validatorList = new ValidatorCollection();
                _validators.Add(parameterIndex, validatorList);
            }
            foreach (BaseValidator validator in validators)
                validatorList.Add(validator);
        }

        public int Count
        {
            get { return _validators.Count; }
        }

        internal IReadOnlyList<BaseValidator> GetValidators(int parameterIndex = -1)
        {
            ValidatorCollection commonValidators;
            _validators.TryGetValue(-1, out commonValidators);

            ValidatorCollection indexValidators = null;
            if (_option.Usage.ParameterType == OptionParameterType.Individual && parameterIndex >= 0)
                _validators.TryGetValue(parameterIndex, out indexValidators);

            int capacity = (commonValidators != null ? commonValidators.Count : 0) + (indexValidators != null ? indexValidators.Count : 0);
            var validators = new List<BaseValidator>(capacity);
            if (commonValidators != null)
                validators.AddRange(commonValidators);
            if (indexValidators != null)
                validators.AddRange(indexValidators);
            return validators;
        }
    }
}
