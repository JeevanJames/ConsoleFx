// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

using Spectre.Console;

namespace ConsoleFx.Prompter
{
    [DebuggerDisplay("Static Text: {Message,nq}")]
    public class StaticText : DisplayItem
    {
        public StaticText(Factory<string> message)
            : base(message)
        {
        }

        public virtual void Display(dynamic answers)
        {
            string staticText = Message.Resolve(answers);
            if (staticText is not null)
                AnsiConsole.MarkupLine(staticText);
        }

        public static StaticText BlankLine() => new(string.Empty);

        public static StaticText Separator(char separatorChar = '=')
        {
            return new StaticText(new string(separatorChar, Console.WindowWidth));
        }
    }
}
