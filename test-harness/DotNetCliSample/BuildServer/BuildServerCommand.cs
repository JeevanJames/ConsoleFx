using System;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

namespace ConsoleFx.TestHarness.DotNetCliSample.BuildServer
{
    [Command("build-server")]
    [CommandHelp("Interact with servers started by a build.")]
    public sealed class BuildServerCommand : AbstractCommand
    {
    }

    [Command("shutdown", typeof(BuildServerCommand))]
    [CommandHelp("Shuts down build servers that are started from dotnet. By default, all servers are shut down.")]
    public sealed class ShutdownCommand : Command
    {
        /// <inheritdoc />
        protected override int HandleCommand()
        {
            Console.WriteLine("Shutting down build server");
            return 0;
        }
    }
}
