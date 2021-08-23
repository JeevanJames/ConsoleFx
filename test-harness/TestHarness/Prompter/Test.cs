#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ConsoleFx.Prompter;
using ConsoleFx.Prompter.Questions;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness.Prompter
{
    internal sealed class Test : TestBase
    {
        private static readonly string NameInstructions1 = "We need your name to address you for the rest of the questions.";
        private static readonly string NameInstructions2 = $"You have {Red.BgWhite}our guarantee{Reset.BgReset} that we will keep your details private.";
        private static readonly string NameInstructions3 = "Please trust us.";

        private static readonly string PasswordInstructions = "We need your password to log into your bank account and steal all your money. Make sure to type it in correctly.";

        internal override async Task RunAsync()
        {
            PrompterFlow.Style = Styling.Ruby;
            var prompter = new PrompterFlow()
                .Input("Name", $"Hi, what's your {Green.BgDkYellow}name? ", q => q
                    .WithInstructions(NameInstructions1, NameInstructions2, NameInstructions3)
                    .ValidateWith(name => name.Length >= 6 ? ValidationResult.Valid : "Enter a name of length 6 or greater")
                    .Transform(name => name.ToUpperInvariant())
                    .DefaultsTo("Jeevan"))
                .UpdateFlow(ans =>
                {
                    if (ans.Name == "JEEVAN")
                    {
                        var surnameQuestion = new InputQuestion("Surname", "What's your surname? ")
                            .ValidateWith(sn => sn.Length >= 5);
                        return new FlowUpdateAction[]
                        {
                            new AddQuestionAction(surnameQuestion, AddLocation.BeforeItem, "Password"),
                        };
                    }

                    return Enumerable.Empty<FlowUpdateAction>();
                })
                .AsyncUpdateFlow(async ans =>
                {
                    string[] lines = await File.ReadAllLinesAsync("./Prompter/DynamicQuestions.txt");
                    List<FlowUpdateAction> actions = new(lines.Length);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('=', 2);
                        actions.Add(new AddQuestionAction(new InputQuestion(parts[0], parts[1]), AddLocation.BeforeItem, "Password"));
                    }

                    return actions;
                })
                .Password("Password", "Enter password: ", q => q
                    .WithInstructions(PasswordInstructions)
                    .ValidateInputWith(password => password.Length > 0))
                .Confirm("Proceed", "Should we proceed? ", @default: false, q => q
                    .WithInstructions("This is a confirmation for YES or NO"))
                .List("Proceed2", "Should we proceed (checkbox)? ", new[] { "Yes", "No", "Maybe" },
                    selected => selected == 1, q => q
                        .WithInstructions("This is a confirmation using checkboxes"))
                .Separator()
                .Checkbox("Checkbox", "This is a checkbox", new []{ "Choice 1", "Choice 2", "Choice 3" }, q => q
                    .WithInstructions("Select at least one option")
                    .ValidateWith(list => list.Count > 0))
                .Text("You have decided to proceed", t => t
                    .When(ans => ans.Proceed))
                .Text("You have decided not to proceed", t => t
                    .When(ans => !ans.Proceed));
            prompter.BetweenPrompts += (_, _) => PrintBlank();
            dynamic answers = await prompter.Ask();

            PrintLine($"Your name is {Yellow}{answers.Name} {answers.Surname ?? string.Empty}");
            PrintLine($"Your password is {Red}{answers.Password}");
            PrintLine($"Should proceed: {Blue}{answers.Proceed}");
            PrintLine($"Should proceed 2: {DkBlue.BgWhite}{answers.Proceed2}");
            if (answers.Checkbox is not null)
                PrintLine($"Checkbox: {string.Join(',', answers.Checkbox)}");
        }
    }
}
