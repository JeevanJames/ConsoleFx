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

namespace ConsoleFx.Parser.Validators
{
    /// <summary>
    /// Checks whether the parameter value is 'True' or 'False'. The check is not case sensitive.
    /// </summary>
    public class BooleanValidator : Validator<string>
    {
        private readonly BooleanType _booleanType;

        public BooleanValidator()
        {
            _booleanType = BooleanType.TrueFalse;
        }

        public BooleanValidator(BooleanType booleanType)
        {
            _booleanType = booleanType;
        }

        public string Message { get; set; } = Messages.Boolean;

        protected override string PrimaryChecks(string parameterValue)
        {
            switch (_booleanType)
            {
                case BooleanType.TrueFalse:
                    bool boolValue;
                    if (!bool.TryParse(parameterValue, out boolValue))
                        ValidationFailed(parameterValue, Message);
                    break;
                case BooleanType.YesNo:
                    if (!parameterValue.Equals("yes", StringComparison.OrdinalIgnoreCase) && !parameterValue.Equals("no", StringComparison.OrdinalIgnoreCase))
                        ValidationFailed(parameterValue, Message);
                    break;
                case BooleanType.OneZero:
                    if (!parameterValue.Equals("1", StringComparison.OrdinalIgnoreCase) && !parameterValue.Equals("0", StringComparison.OrdinalIgnoreCase))
                        ValidationFailed(parameterValue, Message);
                    break;
            }
            return parameterValue;
        }
    }

    public enum BooleanType
    {
        TrueFalse,
        YesNo,
        OneZero,
    }
}