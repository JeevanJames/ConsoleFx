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
    public sealed class EnumValidatorTests
    {
        [Theory]
        [InlineData("Monday")]
        [InlineData("tUESDAY")]
        [InlineData("WeDnEsDaY")]
        [InlineData("saturday")]
        public void Valid_values(string value)
        {
            var validator = new EnumValidator(typeof(DayOfWeek), true);

            Should.NotThrow(() => validator.Validate(value));
        }

        [Theory]
        [InlineData("Monday")]
        [InlineData("Tuesday")]
        public void Valid_values_case_sensitive(string value)
        {
            var validator = new EnumValidator(typeof(DayOfWeek), ignoreCase: false);

            Should.NotThrow(() => validator.Validate(value));
        }

        [Theory]
        [InlineData("")]
        [InlineData("ConsoleFx")]
        public void Invalid_values(string value)
        {
            var validator = new EnumValidator(typeof(DayOfWeek));

            Should.Throw<ValidationException>(() => validator.Validate(value));
        }
    }
}
