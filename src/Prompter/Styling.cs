// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

using ConsoleFx.Prompter.Style;

namespace ConsoleFx.Prompter
{
    public sealed partial class Styling
    {
        private QuestionStyle _question;
        private InstructionStyle _instructions;

        public QuestionStyle Question
        {
            get => _question ??= new QuestionStyle();
            set => _question = value;
        }

        public InstructionStyle Instructions
        {
            get => _instructions ??= new InstructionStyle();
            set => _instructions = value;
        }
    }

    // Themes
    public sealed partial class Styling
    {
        public static readonly Styling NoTheme = new();

        public static readonly Styling Terminal = new()
        {
            Question = { ForeColor = ConsoleColor.Green, },
            Instructions = { ForeColor = ConsoleColor.DarkGreen, },
        };

        public static readonly Styling Ruby = new()
        {
            Question = { ForeColor = ConsoleColor.Magenta, },
        };

        public static readonly Styling Colorful = new()
        {
            Question =
            {
                ForeColor = ConsoleColor.Yellow,
                BackColor = ConsoleColor.DarkMagenta,
            },
            Instructions =
            {
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.DarkBlue,
            },
        };
    }
}
