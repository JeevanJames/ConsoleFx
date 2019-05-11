using System;
using System.Collections.Generic;
using System.Linq;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        private static void Main()
        {
            ColorReset = ConsoleFx.ConsoleExtensions.ColorResetOption.ResetAfterCommand;

            try
            {
                PrintLine($"What do you want to test?");

                string[] menuItems = MenuItems.Values.ToArray();
                int selectedItem = SelectSingle(menuItems);

                Type[] testTypes = MenuItems.Keys.ToArray();
                Type selectedType = testTypes[selectedItem];
                if (selectedType == typeof(Program))
                    return;

                PrintBlank();

                var testHarness = (TestBase)Activator.CreateInstance(selectedType);
                testHarness.Run();
            }
            catch (Exception ex)
            {
                PrintLine($"{Red.BgWhite}[{ex.GetType().Name}]{ex.Message}");
                PrintLine($"{Magenta.BgWhite}{ex.StackTrace}");
            }

            PrintBlank();
            PrintLine("Press any key to escape");
            WaitForAnyKey();
        }

        private static readonly Dictionary<Type, string> MenuItems = new Dictionary<Type, string>
        {
            [typeof(ParserTest)] = "Parser",
            [typeof(ProgramTest)] = "Console Program",
            [typeof(PrompterTest)] = "Prompter",
            [typeof(Program)] = "Exit",
        };
    }
}
