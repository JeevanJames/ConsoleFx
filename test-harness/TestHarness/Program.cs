// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.ConsoleExtensions;

using Spectre.Console;

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

                    string selectedValue = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .Title("What do you want to test?")
                        .AddChoices(MenuItems.Values));

                    Type selectedType = MenuItems.Single(mi => mi.Value.Equals(selectedValue, StringComparison.Ordinal)).Key;
                    if (selectedType == typeof(Program))
                        Environment.Exit(0);

                    Console.WriteLine();

                    var testHarness = (TestBase)Activator.CreateInstance(selectedType);
                    await testHarness.RunAsync();
                }
                catch (TargetInvocationException ex) when (ex.InnerException is not null)
                {
                    AnsiConsole.WriteException(ex.InnerException);
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex);
                }

                Console.WriteLine();
                AnsiConsole.MarkupLine("[black on yellow]Press ANY key to continue...[/]");
                ConsoleEx.WaitForAnyKey();
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = false;
            Process.GetCurrentProcess().Kill();
        }

        private static readonly Dictionary<Type, string> MenuItems = new()
        {
            [typeof(ConsoleProgramTest.Test)] = "Console Program",
            [typeof(DeclarativeConsoleProgramTest.Test)] = "Declarative Console Program",
            [typeof(MultiCommandProgramTest)] = "Multi-command Console Program",
            [typeof(DeepMultiCommand.Test)] = "Deep multi-command Console Program",
            [typeof(Prompter.Test)] = "Prompter",
            [typeof(ConsoleCaptureTest.Test)] = "Console Capture",
            [typeof(Program)] = "Exit",
        };
    }
}
