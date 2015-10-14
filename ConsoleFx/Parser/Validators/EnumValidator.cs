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

using System;
using System.Linq;

namespace ConsoleFx.Parser.Validators
{
    public class EnumValidator : Validator<string>
    {
        private Type EnumType { get; }
        private bool IgnoreCase { get; }

        public EnumValidator(Type enumType, bool ignoreCase = true)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("The enumType parameter should specify a enumerator type", nameof(enumType));
            EnumType = enumType;
            IgnoreCase = ignoreCase;
        }

        public string Message { get; set; } = Messages.Enum;

        protected override string PrimaryChecks(string parameterValue)
        {
            string[] enumNames = Enum.GetNames(EnumType);
            StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!enumNames.Any(enumName => parameterValue.Equals(enumName, comparison)))
                ValidationFailed(parameterValue, Message);
            return parameterValue;
        }
    }

    public class EnumValidator<TEnum> : EnumValidator
        where TEnum : struct
    {
        public EnumValidator(bool ignoreCase = true)
            : base(typeof(TEnum), ignoreCase)
        {
        }
    }
}