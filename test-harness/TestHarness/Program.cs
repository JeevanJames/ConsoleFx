#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        private static async Task Main()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            DebugOutput.Enable();

            Console.CancelKeyPress += Console_CancelKeyPress;

            int selectedItem = 0;
            while (true)
            {
                try
                {
                    Console.Clear();

                    PrintLine("What do you want to test?");

                    string[] menuItems = MenuItems.Values.ToArray();
                    selectedItem = SelectSingle(menuItems, startingIndex: selectedItem);

                    Type[] testTypes = MenuItems.Keys.ToArray();
                    Type selectedType = testTypes[selectedItem];
                    if (selectedType == typeof(Program))
                        Environment.Exit(0);

                    PrintBlank();

                    var testHarness = (TestBase)Activator.CreateInstance(selectedType);
                    await testHarness.RunAsync();
                }
                catch (TargetInvocationException ex) when (ex.InnerException is not null)
                {
                    PrintLine($"{Red.BgWhite}[{ex.InnerException.GetType().Name}]{ex.InnerException.Message}");
                    PrintLine($"{Magenta.BgWhite}{ex.InnerException.StackTrace}");
                }
                catch (Exception ex)
                {
                    PrintLine($"{Red.BgWhite}[{ex.GetType().Name}]{ex.Message}");
                    PrintLine($"{Magenta.BgWhite}{ex.StackTrace}");
                }

                PrintBlank();
                PrintLine($"{Black.BgYellow}Press ANY key to continue...");
                WaitForAnyKey();
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = false;
            Process.GetCurrentProcess().Kill();
        }

        private static readonly Dictionary<Type, string> MenuItems = new()
        {
            [typeof(Parser.Test)] = "Parser",
            [typeof(Parser2.Test)] = "Parser 2",
            [typeof(ConsoleProgramTest.Test)] = "Console Program",
            [typeof(DeclarativeConsoleProgramTest.Test)] = "Declarative Console Program",
            [typeof(ConsoleExtensions.Test)] = "ConsoleEx",
            [typeof(MultiCommandProgramTest)] = "Multi-command Console Program",
            [typeof(DeepMultiCommand.Test)] = "Deep multi-command Console Program",
            [typeof(ProgressBarTest.Test)] = "Progress bar",
            [typeof(Prompter.Test)] = "Prompter",
            [typeof(ConsoleCaptureTest.Test)] = "Console Capture",
            [typeof(Program)] = "Exit",
        };
    }
}
