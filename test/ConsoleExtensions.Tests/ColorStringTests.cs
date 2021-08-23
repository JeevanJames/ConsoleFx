// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
            ColorString cs1 = new ColorString("ConsoleFx ")
                .BgGreen("CLI ")
                .BgBlue("Library ")
                .Yellow().BgBlack("Suite");
            yield return new object[] { cs1, "ConsoleFx [BgGreen]CLI [BgBlue]Library [Yellow.BgBlack]Suite" };

            ColorString cs2 = new ColorString("ConsoleFx ")
                .Green("CLI ")
                .Reset().BgBlue("Library ")
                .Yellow().BgBlack("Suite");
            yield return new object[] { cs2, "ConsoleFx [Green]CLI [Reset.BgBlue]Library [Yellow.BgBlack]Suite" };
        }

        [Fact]
        public void TryParse_ParsesValidColorString()
        {
            const string colorString = "ConsoleFx [Red.BgWhite]Suite";

            bool parseSuccessful = ColorString.TryParse(colorString, out ColorString cstr);

            parseSuccessful.ShouldBeTrue();
            cstr.ShouldNotBeNull();
        }
    }
}
