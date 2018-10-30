#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

namespace ConsoleFx.CmdLineParser.Validators
{
    public sealed class StringValidator : Validator<string>
    {
        public StringValidator(int maxLength)
            : this(0, maxLength)
        {
        }

        public StringValidator(int minLength, int maxLength)
        {
            if (minLength > maxLength)
                throw new ArgumentException("Minimum string length cannot be greater than maximum string length");

            MinLength = minLength;
            MaxLength = maxLength;
        }

        protected override string ValidateAsString(string parameterValue)
        {
            if (parameterValue.Length < MinLength)
                ValidationFailed(MinLengthMessage, parameterValue, MinLength);
            if (parameterValue.Length > MaxLength)
                ValidationFailed(MaxLengthMessage, parameterValue, MaxLength);
            return parameterValue;
        }

        public int MaxLength { get; }

        public int MinLength { get; }

        public string MinLengthMessage { get; set; } = Messages.String_MinLength;

        public string MaxLengthMessage { get; set; } = Messages.String_MaxLength;
    }
}