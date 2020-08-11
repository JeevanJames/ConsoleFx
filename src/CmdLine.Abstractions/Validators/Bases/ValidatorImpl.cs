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

namespace ConsoleFx.CmdLine.Validators.Bases
{
    internal abstract class ValidatorImpl : IValidator
    {
        public Type ExpectedType { get; }

        public object Value { get; private set; }

        public virtual void Validate(string parameterValue)
        {
            Value = ValidateAsString(parameterValue);

            // If there is a value, check that it is compatible with the expected type.
#pragma warning disable S2219 // Runtime type checking should be simplified
            if (Value != null && !ExpectedType.IsAssignableFrom(Value.GetType()))
#pragma warning restore S2219 // Runtime type checking should be simplified
                throw new InvalidOperationException($"The {GetType().FullName} validator should validate an arg as a {ExpectedType.FullName} type, but is validating as a {Value.GetType().FullName} type.");

            // If the type is not a string, call the ValidateAsActualType method to perform additional
            // validations.
            if (ExpectedType != typeof(string))
                ValidateAsActualType(Value, parameterValue);
        }

        protected abstract object ValidateAsString(string parameterValue);

        protected virtual void ValidateAsActualType(object value, string parameterValue)
        {
        }
    }
}