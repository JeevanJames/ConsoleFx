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
using System.Diagnostics;
using System.Reflection;

namespace ConsoleFx.CmdLine.Validators.Bases
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ValidatorAttribute : Attribute, IArgApplicator<Argument>, IArgApplicator<Option>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ConstructorInfo _constructor;

        protected ValidatorAttribute(Type validatorType, object[] args, Type[] argTypes)
        {
            if (validatorType is null)
                throw new ArgumentNullException(nameof(validatorType));
            if (!typeof(Validator).IsAssignableFrom(validatorType))
                throw new ArgumentException("Specified type is not a validator.", nameof(validatorType));

#if NET45
            if (argTypes is null)
                argTypes = new Type[0];
            Args = args ?? new object[0];
#else
            if (argTypes is null)
                argTypes = Array.Empty<Type>();
            Args = args ?? Array.Empty<object>();
#endif

            if (argTypes.Length != Args.Length)
                throw new ArgumentException("Number of arguments does not match number of argument types.", nameof(args));

            _constructor = validatorType.GetConstructor(argTypes);
            if (_constructor is null)
                throw new ArgumentException($"Cannot find constructor in '{validatorType.FullName}' that matches the specified arguments.", nameof(args));
        }

        public Type ValidatorType { get; }

        public object[] Args { get; }

        //TODO: Are these 2 methods being used?
        public void Apply(Argument arg)
        {
            arg.Validators.Add(CreateValidator());
        }

        public void Apply(Option arg)
        {
            arg.Validators.Add(CreateValidator());
        }

        private Validator CreateValidator()
        {
            return (Validator)_constructor.Invoke(Args);
        }
    }
}
