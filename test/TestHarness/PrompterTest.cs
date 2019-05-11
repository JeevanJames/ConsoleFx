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

using ConsoleFx.Prompter;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal sealed class PrompterTest : TestBase
    {
        internal override void Run()
        {
            var prompter = new Prompter()
                .Input("Name", "Hi, what's your name? ", q => q
                    .When(ans => true)
                    .ValidateWith((name, _) => name.Length >= 6)
                    .Transform(name => name.ToUpperInvariant())
                    .DefaultsTo("Jeevan"))
                .Password("Password", "Enter password: ")
                .Confirm("Proceed", "Should we proceed? ", true)
                .List<bool>("Proceed2", "Should we proceed (checkbox)? ", new[] { "Yes", "No" },
                    selected => selected == 0)
                .Text("You have decided to proceed", t => t
                    .When(ans => ans.Proceed))
                .Text("You have decided not to proceed", t => t
                    .When(ans => !ans.Proceed));
            prompter.BetweenPrompts += (sender, args) => PrintBlank();
            dynamic answers = prompter.Ask();

            PrintLine($"Your name is {Yellow}{answers.Name}");
            PrintLine($"Your password is {Red}{answers.Password}");
            PrintLine($"Should proceed: {Blue}{answers.Proceed}");
            PrintLine($"Should proceed 2: {DkBlue.BgWhite}{answers.Proceed2}");
        }
    }
}
