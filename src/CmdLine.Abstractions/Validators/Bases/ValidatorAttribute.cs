// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace ConsoleFx.CmdLine.Validators.Bases
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ValidatorAttribute : Attribute, IArgApplicator<Argument>, IArgApplicator<Option>, IValidator
    {
        private readonly IValidator _validatorImpl;

        protected ValidatorAttribute(IValidator impl)
        {
            _validatorImpl = impl;
        }

        public Type ExpectedType => _validatorImpl.ExpectedType;

        public object Value => _validatorImpl.Value;

        public void Validate(string parameterValue)
        {
            _validatorImpl.Validate(parameterValue);
        }

        void IArgApplicator<Argument>.Apply(Argument arg, PropertyInfo property)
        {
            arg.Validators.Add(this);
        }

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo property)
        {
            arg.Validators.Add(this);
        }
    }
}
