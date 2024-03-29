﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Collection of all parameter validators of an option. The collection is grouped by parameter index.
    ///     <para />
    ///     A parameter index of -1 indicates validators for all parameters.
    /// </summary>
    [DebuggerDisplay("Option {_option.Name} validators")]
    public sealed class OptionParameterValidators
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Option _option;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<int, ValidatorCollection> _validators = new();

        internal OptionParameterValidators(Option option)
        {
            _option = option ?? throw new ArgumentNullException(nameof(option));
        }

        /// <summary>
        ///     Adds one or more validators that apply to all parameters.
        /// </summary>
        /// <param name="validators">Collection of parameters to add.</param>
        public void Add(params IValidator[] validators)
        {
            Add(-1, validators);
        }

        /// <summary>
        ///     Adds one or more validators that apply to the parameter at the specified parameter index.
        /// </summary>
        /// <param name="parameterIndex">Zero-based index of the parameter to apply the validators to.</param>
        /// <param name="validators">Collection of parameters to add.</param>
        public void Add(int parameterIndex, params IValidator[] validators)
        {
            //TODO: These checks on the option usage can fail incorrectly if the validators are set
            //before the usage is set. Better to do these checks when the parser is called rather than
            //now.

            if (_option.Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
            {
                throw new ParserException(1000,
                    $"Cannot add validators to option {_option.Name} because it does not accept parameters.");
            }

            if (parameterIndex >= 0 && _option.Usage.ParameterType == OptionParameterType.Repeating)
            {
                throw new ParserException(1000,
                    $"Cannot add a specific parameter validator for option {_option.Name} because it is configured to have repeating parameters, where all parameters share the same validators. Use the ValidateWith overload that does not accept a parameter index.");
            }

            if (parameterIndex >= _option.Usage.MaxParameters)
            {
                throw new ParserException(1000,
                    $"Parameter index specified {parameterIndex} is greater than the number of parameters allowed for option {_option.Name}.");
            }

            if (parameterIndex < -1)
                parameterIndex = -1;

            if (!_validators.TryGetValue(parameterIndex, out ValidatorCollection validatorList))
            {
                validatorList = new ValidatorCollection();
                _validators.Add(parameterIndex, validatorList);
            }

            foreach (IValidator validator in validators)
                validatorList.Add(validator);
        }

        /// <summary>
        ///     Gets the total number of validators across all parameter indices.
        /// </summary>
        public int Count => _validators.Sum(kvp => kvp.Value.Count);

        /// <summary>
        ///     Gets the total number of validators for the parameter at the specified index.
        /// </summary>
        /// <param name="parameterIndex">The index of the parameter to get the validator count.</param>
        /// <returns>Total number of validators for the parameter at the specified index.</returns>
        public int? CountOf(int parameterIndex)
        {
            return _validators.TryGetValue(parameterIndex, out ValidatorCollection validators) ? validators.Count : null;
        }

        /// <summary>
        ///     Gets the collection of validators for the parameter at the specified index.
        /// </summary>
        /// <param name="parameterIndex">
        ///     Index of the parameter to get the validator collection. Use -1 to get the validators for
        ///     all parameters.
        /// </param>
        /// <returns>The validators for the parameters at the specified index.</returns>
        public IEnumerable<IValidator> GetValidators(int parameterIndex)
        {
            _validators.TryGetValue(-1, out ValidatorCollection commonValidators);

            if (_option.Usage.ParameterType != OptionParameterType.Individual || parameterIndex < 0)
                return commonValidators ?? Enumerable.Empty<IValidator>();

            if (!_validators.TryGetValue(parameterIndex, out ValidatorCollection indexValidators) || indexValidators is null)
                return commonValidators ?? Enumerable.Empty<IValidator>();

            return (commonValidators ?? Enumerable.Empty<IValidator>()).Concat(indexValidators);
        }
    }
}
