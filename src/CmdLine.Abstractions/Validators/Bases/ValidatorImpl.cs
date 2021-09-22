// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
            if (Value is not null && !ExpectedType.IsAssignableFrom(Value.GetType()))
#pragma warning restore S2219 // Runtime type checking should be simplified
                throw new InvalidOperationException($"The {GetType()} validator should validate an arg as a {ExpectedType} type, but is validating as a {Value.GetType()} type.");

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
