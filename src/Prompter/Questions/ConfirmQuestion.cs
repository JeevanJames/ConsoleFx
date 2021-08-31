// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class ConfirmQuestion : Question<bool, bool>
    {
        private readonly bool _default;

        public ConfirmQuestion(string name, FunctionOrColorString message, bool @default)
            : base(name, message)
        {
            _default = @default;
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            ConsoleEx.Print(new ColorString(Message.Resolve(answers),
                PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));

            ConsoleEx.Print(new ColorString($" ({(_default ? 'Y' : 'y')}/{(_default ? 'n' : 'N')}) ",
                PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));

            ConsoleKey keyPressed = ConsoleEx.WaitForKeys(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.Enter);
            bool result = keyPressed == ConsoleKey.Enter ? _default : (keyPressed == ConsoleKey.Y);

            ConsoleEx.PrintLine(result ? "Y" : "N");

            return result;
        }
    }
}
