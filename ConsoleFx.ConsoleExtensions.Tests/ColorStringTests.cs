using System;
using Xunit;

namespace ConsoleFx.ConsoleExtensions.Tests
{
    public sealed class ColorStringTests
    {
        [Fact]
        public void ToString_builds_color_string()
        {
            var cs = new ColorString();
            string cstr = cs.ToString();

            Assert.NotNull(cstr);
            Assert.NotEmpty(cstr);
        }
    }
}
