using ConsoleFx.Prompter;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal sealed class PrompterTest : TestBase
    {
        internal override void Run()
        {
            ColorReset = ConsoleFx.ConsoleExtensions.ColorResetOption.ResetAfterCommand;

            var prompter = new Prompter()
                .Input("Name", "Hi, what's your name? ", q => q
                    .When(ans => true)
                    .ValidateWith((name, _) => name.Length >= 6)
                    .Transform(name => name.ToUpperInvariant())
                    .DefaultsTo("Jeevan"))
                .Password("Password", "Enter password: ")
                .Confirm("Proceed", "Should we proceed? ", true)
                .List<bool>("Proceed2", "Should we proceed (checkbox)? ", new[] { "Yes", "No" }, q => q
                    .Transform(selected => selected == 0))
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
