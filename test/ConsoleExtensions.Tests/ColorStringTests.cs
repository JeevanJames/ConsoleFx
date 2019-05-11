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

using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace ConsoleFx.ConsoleExtensions.Tests
{
    public sealed class ColorStringTests
    {
        [Theory]
        [MemberData(nameof(ToString_BuildsColorString_Data))]
        public void ToString_BuildsColorString(ColorString cs, string expectedString)
        {
            string colorString = cs.ToString();

            colorString.ShouldBe(expectedString);
        }

        public static IEnumerable<object[]> ToString_BuildsColorString_Data()
        {
            var cs1 = new ColorString("ConsoleFx ")
                .BgGreen("CLI ")
                .BgBlue("Library ")
                .Yellow().BgBlack("Suite");
            yield return new object[] { cs1, "ConsoleFx [BgGreen]CLI [BgBlue]Library [Yellow.BgBlack]Suite" };

            var cs2 = new ColorString("ConsoleFx ")
                .Green("CLI ")
                .Reset().BgBlue("Library ")
                .Yellow().BgBlack("Suite");
            yield return new object[] { cs2, "ConsoleFx [Green]CLI [Reset.BgBlue]Library [Yellow.BgBlack]Suite" };
        }

        [Fact]
        public void TryParse_ParsesValidColorString()
        {
            const string colorString = @"ConsoleFx [Red.BgWhite]Suite";

            bool parseSuccessful = ColorString.TryParse(colorString, out ColorString cstr);

            parseSuccessful.ShouldBeTrue();
            cstr.ShouldNotBeNull();
        }
    }
}
