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

using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Prompter;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal sealed class PrompterTest : TestBase
    {
        private const string NameInstructions1 = @"We need your name to address you for the rest of the questions.";
        private readonly string NameInstructions2 = $@"You have {Red.BgWhite}our guarantee{Reset.BgReset} that we will keep your details private.";
        private const string NameInstructions3 = @"Please trust us.";

        private const string PasswordInstructions = @"We need your password to log into your bank account and steal all your money. Make sure to type it in correctly.";

        internal override void Run()
        {
            Prompter.Style = Styling.Colorful;
            var prompter = new Prompter()
                .Input("Name", $"Hi, what's your {Green.BgDkYellow}name? ", q => q
                    .WithInstructions(NameInstructions1, NameInstructions2, NameInstructions3)
                    .ValidateWith((name, _) => name.Length >= 6)
                    .Transform(name => name.ToUpperInvariant())
                    .DefaultsTo("Jeevan"))
                .Password("Password", "Enter password: ", q => q
                    .WithInstructions(PasswordInstructions)
                    .ValidateInputWith((password, _) => password.Length > 0))
                .Confirm("Proceed", "Should we proceed? ", true)
                .List("Proceed2", "Should we proceed (checkbox)? ", new[] { "Yes", "No" },
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
