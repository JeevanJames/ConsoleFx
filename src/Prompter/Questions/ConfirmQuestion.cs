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
            AskerFn = (q, ans) =>
            {
                ConsoleEx.Print(new ColorString(q.Message.Resolve(ans),
                    PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));

                var cq = (ConfirmQuestion)q;
                ConsoleEx.Print(new ColorString($" ({(cq._default ? 'Y' : 'y')}/{(cq._default ? 'n' : 'N')}) ",
                    PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor));

                ConsoleKey keyPressed = ConsoleEx.WaitForKeys(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.Enter);
                bool result = keyPressed == ConsoleKey.Enter ? cq._default : (keyPressed == ConsoleKey.Y);

                ConsoleEx.PrintLine(result ? "Y" : "N");

                return result;
            };
            _default = @default;
        }

        internal override AskerFn AskerFn { get; }
    }
}
