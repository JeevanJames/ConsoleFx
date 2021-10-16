// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ConsoleFx.Prompter;
using ConsoleFx.Prompter.Questions;

using Spectre.Console;

namespace TestHarness.Prompter
{
    internal sealed class Test : TestBase
    {
        private const string NameInstructions1 = "We need your name to address you for the rest of the questions.";
        private const string NameInstructions3 = "Please trust us.";
        private const string PasswordInstructions = "We need your password to log into your bank account and steal all your money. Make sure to type it in correctly.";

        private static readonly string NameInstructions2 = "You have [red on white]our guarantee[/] that we will keep your details private.";

        internal override async Task RunAsync()
        {
            PrompterFlow.Style = Styling.Ruby;
            var prompter = new PrompterFlow
            {
                new InputQuestion("Name", "Hi, what's your [green on olive]name?[/] ")
                    .WithInstructions(NameInstructions1, NameInstructions2, NameInstructions3)
                    .ValidateWith(name =>
                        name.Length >= 6 ? ValidationResult.Success() : ValidationResult.Error("Enter a name of length 6 or greater"))
                    .Transform(name => name.ToUpperInvariant())
                    .DefaultsTo("Jeevan"),
                new UpdateFlowItem(ans =>
                {
                    if (ans.Name == "JEEVAN")
                    {
                        Question<string, string> surnameQuestion = new InputQuestion("Surname", "What's your surname? ")
                            .ValidateWith(sn => sn.Length >= 5 ? ValidationResult.Success() : ValidationResult.Error())
                            .DefaultsTo("James");
                        return new FlowUpdateAction[]
                        {
                            new AddItemAction(surnameQuestion),
                        };
                    }

                    return Enumerable.Empty<FlowUpdateAction>();
                }),
                new ConfirmQuestion("AdditionalQuestions", "Do you want to load additional questions? ",
                    @default: true),
                new UpdateFlowItem(async ans =>
                {
                    string[] lines = await File.ReadAllLinesAsync("./Prompter/DynamicQuestions.txt");
                    List<FlowUpdateAction> actions = new(lines.Length);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('=', 2);
                        Question<string, string> inputQuestion = new InputQuestion(parts[0], parts[1])
                            .ValidateWith(s => s.Length > 0 ? ValidationResult.Success() : ValidationResult.Error());
                        actions.Add(new AddItemAction(inputQuestion));
                    }

                    return actions;
                }).When(ans => ans.AdditionalQuestions),
                new PasswordQuestion("Password", "Enter password: ")
                    .WithInstructions(PasswordInstructions)
                    .ValidateInputWith(password => password.Length > 0 ? ValidationResult.Success() : ValidationResult.Error()),
                new ConfirmQuestion("Proceed", "Should we proceed? ", @default: false)
                    .WithInstructions("This is a confirmation for YES or NO"),
                new ListQuestion<bool>("Proceed2", "Should we proceed (checkbox)? ", new[] { "Yes", "No", "Maybe" })
                    .WithInstructions("This is a confirmation using checkboxes")
                    .Transform(selected => selected == 1),
                StaticText.Separator(),
                new CheckboxQuestion("Checkbox", "This is a checkbox", new[] { "Choice 1", "Choice 2", "Choice 3" })
                    .WithInstructions("Select at least one option")
                    .ValidateWith(list => list.Count > 0 ? ValidationResult.Success() : ValidationResult.Error()),
                new StaticText("You have decided to proceed").When(ans => ans.Proceed),
                new StaticText("You have decided not to proceed").When(ans => !ans.Proceed),
            };
            dynamic answers = await prompter.Ask();

            AnsiConsole.MarkupLine($"Your name is [yellow]{answers.Name} {answers.Surname ?? string.Empty}[/]");
            AnsiConsole.MarkupLine($"Your password is [red]{answers.Password}[/]");
            AnsiConsole.MarkupLine($"Should proceed: [blue]{answers.Proceed}[/]");
            AnsiConsole.MarkupLine($"Should proceed 2: [darkblue on white]{answers.Proceed2}[/]");
            if (answers.Checkbox is not null)
                AnsiConsole.MarkupLine($"Checkbox: {string.Join(',', answers.Checkbox)}");
        }
    }
}
