// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using ConsoleFx.ConsoleExtensions;
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
            Question = { ForeColor = CColor.Green, },
            Instructions = { ForeColor = CColor.DkGreen, },
        };

        public static readonly Styling Ruby = new()
        {
            Question = { ForeColor = CColor.Magenta, },
        };

        public static readonly Styling Colorful = new()
        {
            Question =
            {
                ForeColor = CColor.Yellow,
                BackColor = CColor.DkMagenta,
            },
            Instructions =
            {
                ForeColor = CColor.White,
                BackColor = CColor.DkBlue,
            },
        };
    }
}
