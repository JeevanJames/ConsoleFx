using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

namespace ConsoleFx.TestHarness.DotNetCliSample.Clean
{
    [Command("clean")]
    [CommandHelp("Clean build outputs of a .NET project.")]
    public sealed class CleanCommand : Command
    {
    }
}
