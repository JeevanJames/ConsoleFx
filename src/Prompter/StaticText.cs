// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Claims;

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    [DebuggerDisplay("Static Text: {Message,nq}")]
    public sealed class StaticText : DisplayItem
    {
        public StaticText(FunctionOrColorString message)
            : base(message)
        {
            AskerFn = (q, ans) =>
            {
                ColorString staticText = q.Message.Resolve(ans);
                if (staticText is not null)
                    ConsoleEx.PrintLine(staticText);
                return null;
            };
        }

        internal override AskerFn AskerFn { get; }

        public static StaticText BlankLine() => new(ColorString.Empty);

        public static StaticText Separator(char separatorChar = '=')
        {
            return new StaticText(new ColorString(new string(separatorChar, Console.WindowWidth)));
        }
    }
}
