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

using ConsoleFx.CmdLine.Validators;

using Shouldly;

using Xunit;

namespace CmdLine.Abstractions.Tests.Validators
{
    public sealed class GuidValidatorTest
    {
        private readonly GuidValidator _validator = new GuidValidator();

        [Theory]
        [InlineData("{93FBAE5A-C158-411A-A7C8-2BCF5AFB3857}")]
        [InlineData("93FBAE5A-C158-411A-A7C8-2BCF5AFB3857")]
        [InlineData("93FBAE5AC158411AA7C82BCF5AFB3857")]
        public void Valid_values(string value)
        {
            Should.NotThrow(() => _validator.Validate(value));
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData("This is not a GUID")]
        [InlineData("0123456789")]
        public void Invalid_values(string value)
        {
            Should.Throw<ValidationException>(() => _validator.Validate(value));
        }

        [Fact]
        public void Throws_ArgumentNullException_on_null_value()
        {
            Should.Throw<ArgumentNullException>(() => _validator.Validate(null));
        }
    }
}
