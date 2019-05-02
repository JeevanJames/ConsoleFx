using System.Collections.Generic;

using Shouldly;

using Xunit;

using static ConsoleFx.ConsoleExtensions.Clr;

namespace ConsoleFx.ConsoleExtensions.Tests
{
    public sealed class ClrTests
    {
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
        }
    }
}
