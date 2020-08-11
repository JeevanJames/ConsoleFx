#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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

using ConsoleFx.CmdLine.Validators;

using Shouldly;

using Xunit;

namespace CmdLine.Abstractions.Tests.Validators
{
    public sealed class BooleanValidatorTests
    {
        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("TRUE")]
        [InlineData("FaLsE")]
        public void Valid_values_for_default_validator(string value)
        {
            var validator = new BooleanValidator();

            Should.NotThrow(() => validator.Validate(value));
        }

        [Theory]
        [InlineData("0")]
        [InlineData("Yes")]
        [InlineData("T")]
        public void Invalid_values_for_default_validator(string value)
        {
            var validator = new BooleanValidator();

            Should.Throw<ValidationException>(() => validator.Validate(value));
        }

        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        public void Valid_values_for_default_case_sensitive_validator(string value)
        {
            var validator = new BooleanValidator(caseSensitive: true);

            Should.NotThrow(() => validator.Validate(value));
        }

        [Theory]
        [InlineData("TRUE")]
        [InlineData("FaLsE")]
        public void Invalid_values_for_default_case_sensitive_validator(string value)
        {
            var validator = new BooleanValidator(caseSensitive: true);

            Should.Throw<ValidationException>(() => validator.Validate(value));
        }

        [Theory]
        [InlineData("yes")]
        [InlineData("1")]
        [InlineData("No")]
        [InlineData("f")]
        public void Valid_values_for_custom_validator(string value)
        {
            var validator = new BooleanValidator(new[] { "1", "Yes", "T" }, new[] { "0", "No", "F" });

            Should.NotThrow(() => validator.Validate(value));
        }

        [Theory]
        [InlineData(null, "false", typeof(ArgumentNullException))]
        [InlineData("true", null, typeof(ArgumentNullException))]
        [InlineData("", "false", typeof(ArgumentException))]
        [InlineData("true", "", typeof(ArgumentException))]
        [InlineData("    ", "false", typeof(ArgumentException))]
        [InlineData("true", "    ", typeof(ArgumentException))]
        [InlineData("true", "true", typeof(ArgumentException))]
        public void Invalid_values_for_default_validator_constructor(string trueString, string falseString, Type expectedExceptionType)
        {
            Should.Throw(() => new BooleanValidator(trueString, falseString), expectedExceptionType);
        }

        [Theory]
        [InlineData(null, null, typeof(ArgumentNullException))]
        [InlineData(new[] { "Yes" }, null, typeof(ArgumentNullException))]
        [InlineData(null, new[] { "No" }, typeof(ArgumentNullException))]
        [InlineData(new string[0], new[] { "No" }, typeof(ArgumentException))]
        [InlineData(new[] { "Yes" }, new string[0], typeof(ArgumentException))]
        [InlineData(new string[] { null }, new[] { "No" }, typeof(ArgumentException))]
        [InlineData(new[] { "Yes" }, new string[] { null }, typeof(ArgumentException))]
        [InlineData(new[] { "" }, new[] { "No" }, typeof(ArgumentException))]
        [InlineData(new[] { "Yes" }, new[] { "" }, typeof(ArgumentException))]
        [InlineData(new string[] { "Yes", null }, new[] { "No" }, typeof(ArgumentException))]
        [InlineData(new[] { "Yes" }, new string[] { "No", null }, typeof(ArgumentException))]
        [InlineData(new[] { "Yes", "" }, new[] { "No" }, typeof(ArgumentException))]
        [InlineData(new[] { "Yes" }, new[] { "No", "" }, typeof(ArgumentException))]
        [InlineData(new[] { "Yes", "T" }, new[] { "No", "T" }, typeof(ArgumentException))]
        public void Invalid_custom_values_for_validators(string[] trueValues, string[] falseValues, Type expectedExceptionType)
        {
            Should.Throw(() => new BooleanValidator(trueValues, falseValues), expectedExceptionType);
        }
    }
}
