using System;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                TestBase testHarness = new ParserTest();
                testHarness.Run();
            }
            catch (Exception ex)
            {
                PrintLine($"{Red.BgWhite}[{ex.GetType().Name}]{ex.Message}");
            }

            PrintBlank();
            PrintLine("Press any key to escape");
            WaitForAnyKey();
        }
    }
}
