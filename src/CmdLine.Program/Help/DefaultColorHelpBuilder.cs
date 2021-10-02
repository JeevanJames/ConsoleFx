// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine.Program.Help
{
    public class DefaultColorHelpBuilder : DefaultHelpBuilder
    {
        public DefaultColorHelpBuilder(params string[] names)
            : base(names)
        {
        }

        public DefaultColorHelpBuilder(bool caseSensitive, params string[] names)
            : base(caseSensitive, names)
        {
        }

        protected override void PrintArg(string name, string description, int maxNameLength, ArgDescriptionPlacement placement)
        {
            Console.Write(IndentStr);

            Console.ForegroundColor = ConsoleColor.Magenta;
            if (placement == ArgDescriptionPlacement.SameLine)
            {
                Console.Write(name.PadRight(maxNameLength) + NameDescriptionSpacingStr);
            }
            else
            {
                Console.WriteLine(name);
                Console.Write(NewLineDescriptionIndentStr);
            }

            Console.ResetColor();

            Console.WriteLine(description);
        }
    }
}
