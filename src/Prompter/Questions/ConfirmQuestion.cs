#region --- License & Copyright Notice ---
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
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class ConfirmQuestion : Question<bool, bool>
    {
        private readonly AskerFn _askerFn;
        private readonly bool _default;

        internal ConfirmQuestion(string name, FunctionOrValue<string> message, bool @default)
            : base(name, message)
        {
            _askerFn = (q, ans) =>
            {
                ConsoleEx.Print(new ColorString().Magenta(q.Message.Resolve(ans)));

                var cq = (ConfirmQuestion)q;
                ConsoleEx.Print($"{Clr.Magenta} ({(cq._default ? 'Y' : 'y')}/{(cq._default ? 'n' : 'N')}) ");

                ConsoleKey keyPressed = ConsoleEx.WaitForKeys(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.Enter);
                bool result = keyPressed == ConsoleKey.Enter ? cq._default : (keyPressed == ConsoleKey.Y);

                ConsoleEx.PrintLine(result ? "Y" : "N");

                return result;
            };
            _default = @default;
        }

        internal override AskerFn AskerFn => _askerFn;
    }
}
