﻿#region --- License & Copyright Notice ---
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
            [typeof(MultiCommandProgramTest)] = "Multi-command Console Program",
            [typeof(PrompterTest)] = "Prompter",
            [typeof(Program)] = "Exit",
        };
    }
}
