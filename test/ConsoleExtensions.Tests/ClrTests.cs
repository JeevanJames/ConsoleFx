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

using static ConsoleFx.ConsoleExtensions.Clr;

namespace ConsoleFx.ConsoleExtensions.Tests
{
    public sealed class ClrTests
    {
        [Fact]
        public void Test()
        {
            string str = Yellow.BgBlack.ToString();
            str.ShouldBe("[Yellow.BgBlack]");
        }

        [Theory, MemberData(nameof(ToString_GeneneratesCorrectString_Data))]
        public void ToString_GeneratesCorrectString(InstanceClr clr, string expectedString)
        {
            string actualString = clr.ToString();

            actualString.ShouldBe(expectedString);
        }

        public static IEnumerable<object[]> ToString_GeneneratesCorrectString_Data()
        {
            yield return new object[] { Yellow.BgRed, "[Yellow.BgRed]" };
            yield return new object[] { Yellow, "[Yellow]" };
            yield return new object[] { BgRed, "[BgRed]" };
            yield return new object[] { Yellow.Cyan, "[Cyan]" };
            yield return new object[] { BgRed.BgBlue, "[BgBlue]" };
            yield return new object[] { BgRed.BgBlack.Blue, "[Blue.BgBlack]" };
        }

        [Theory, MemberData(nameof(InterpolatedStringGeneratesCorrectString_Data))]
        public void InterpolatedStringGeneratesCorrectString(string interpolatedString, string expectedString)
        {
            interpolatedString.ShouldBe(expectedString);
        }

        public static IEnumerable<object[]> InterpolatedStringGeneratesCorrectString_Data()
        {
            yield return new object[] { $"ConsoleFx {Cyan.BgYellow}Suite", "ConsoleFx [Cyan.BgYellow]Suite" };
            yield return new object[] { $"{Green.BgBlack}ConsoleFx Suite", "[Green.BgBlack]ConsoleFx Suite" };
            yield return new object[] { $"ConsoleFx Suite{Magenta.BgDkBlue}", "ConsoleFx Suite[Magenta.BgDkBlue]" };
            yield return new object[]
            {
                $"{White.BgBlack}ConsoleFx {Magenta.BgDkBlue}Suite",
                "[White.BgBlack]ConsoleFx [Magenta.BgDkBlue]Suite",
            };
        }
    }
}
