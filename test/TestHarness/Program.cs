using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        private static void Main()
        {
            TestBase testHarness = new ParserTest();
            testHarness.Run();

            PrintBlank();
            PrintLine("Press any key to escape");
            WaitForAnyKey();
        }
    }
}
