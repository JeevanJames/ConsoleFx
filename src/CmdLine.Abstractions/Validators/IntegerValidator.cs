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

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public class IntegerValidator : Validator<long>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly long _minimumValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly long _maximumValue;

        public IntegerValidator(long minimumValue = long.MinValue, long maximumValue = long.MaxValue)
        {
            if (minimumValue > maximumValue)
                throw new ArgumentException("Minimum value cannot be greater than the maximum value.", nameof(minimumValue));
            _minimumValue = minimumValue;
            _maximumValue = maximumValue;
        }

        public string NotAnIntegerMessage { get; set; } = Messages.Integer_NotAnInteger;

        public string OutOfRangeMessage { get; set; } = Messages.Integer_OutOfRange;

        protected override long ValidateAsString(string parameterValue)
        {
            if (!long.TryParse(parameterValue, out long value))
                ValidationFailed(NotAnIntegerMessage, parameterValue);
            if (value < _minimumValue || value > _maximumValue)
                ValidationFailed(OutOfRangeMessage, parameterValue);
            return value;
        }
    }

    public static class IntegerValidatorExtensions
    {
        public static Argument ValidateAsInteger(this Argument argument, long minimumValue = long.MinValue,
            long maximumValue = long.MaxValue, string notAnIntegerMessage = null, string outOfRangeMessage = null)
        {
            var validator = new IntegerValidator(minimumValue, maximumValue);
            if (notAnIntegerMessage != null)
                validator.NotAnIntegerMessage = notAnIntegerMessage;
            if (outOfRangeMessage != null)
                validator.OutOfRangeMessage = outOfRangeMessage;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsInteger(this Option option, long minimumValue = long.MinValue,
            long maximumValue = long.MaxValue, int parameterIndex = -1, string notAnIntegerMessage = null,
            string outOfRangeMessage = null)
        {
            var validator = new IntegerValidator(minimumValue, maximumValue);
            if (notAnIntegerMessage != null)
                validator.NotAnIntegerMessage = notAnIntegerMessage;
            if (outOfRangeMessage != null)
                validator.OutOfRangeMessage = outOfRangeMessage;
            return option.ValidateWith(parameterIndex, validator).TypeAs<long>();
        }
    }
}
