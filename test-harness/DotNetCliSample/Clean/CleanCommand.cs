using ConsoleFx.CmdLine;

namespace ConsoleFx.TestHarness.DotNetCliSample.Clean
{
    [Command("clean", HelpText = "Clean build outputs of a .NET project.")]
    public sealed class CleanCommand : Command
    {
    }
}
