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

            dynamic answers = new Prompter()
                .Input("Name", "Hi, what's your name? ",
                    q => q.When(ans => true).DefaultsTo("Jeevan"))
                .Password("Password", "Enter password: ")
                .Confirm("Proceed", "Should we proceed? ", true)
                .Text("You have decided to proceed",
                    t => t.When(ans => ans.Proceed))
                .Text("You have decided not to proceed",
                    t => t.When(ans => !ans.Proceed))
                .Ask();

            PrintLine($"Your name is {Yellow}{answers.Name}");
            PrintLine($"Your password is {Red}{answers.Password}");
            PrintLine($"Should proceed: {Blue}{answers.Proceed}");
        }
    }
}
