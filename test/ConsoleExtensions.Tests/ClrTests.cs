// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
