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

namespace ConsoleFx.Parser.Validators
{
    public class IntegerValidator : Validator<long>
    {
        private readonly long _minimumValue;
        private readonly long _maximumValue;

        public IntegerValidator(long minimumValue = long.MinValue, long maximumValue = long.MaxValue)
        {
            _minimumValue = minimumValue;
            _maximumValue = maximumValue;
        }

        public string NotAnIntegerMessage { get; set; } = Messages.Integer_NotAnInteger;
        public string OutOfRangeMessage { get; set; } = Messages.Integer_OutOfRange;

        protected override long ValidateAsString(string parameterValue)
        {
            long value;
            if (!long.TryParse(parameterValue, out value))
                ValidationFailed(NotAnIntegerMessage, parameterValue);
            if (value < _minimumValue || value > _maximumValue)
                ValidationFailed(OutOfRangeMessage, parameterValue);
            return value;
        }
    }
}