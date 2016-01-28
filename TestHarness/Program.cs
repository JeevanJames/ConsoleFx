using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

using static ConsoleFx.Utilities.ConsoleEx;

using static System.Console;

namespace TestHarness
{
    public enum BackupType
    {
        Full,
        Incremental
    }

    internal sealed class Parameters
    {
        public bool Verbose { get; set; }
        public BackupType BackupType { get; set; } = BackupType.Full;
        public List<string> Excludes { get; } = new List<string>();
        public DirectoryInfo BackupDirectory { get; set; }
    }

    internal static class Program
    {
        private static int Main()
        {
            var parameters = new Parameters();
            var app = new SimpleProgram(Handler, new WindowsParserStyle(), scope: parameters);
            app.BeforeError += (sender, args) => ForegroundColor = ConsoleColor.Red;
            app.AfterError += (sender, args) => ResetColor();
            app.AddOption("verbose", "v")
                .Flag(() => parameters.Verbose);
            app.AddOption("type", "t")
                .ParametersRequired()
                .ValidateWith(new EnumValidator<BackupType> {
                    Message = "Please specify either Full or Incremental for the backup type."
                })
                .AssignTo(() => parameters.BackupType);
            app.AddOption("exclude", "e")
                .Optional(int.MaxValue)
                .ParametersRequired(int.MaxValue)
                .ValidateWith(new RegexValidator(@"^[\w.*?]+$"))
                .AddToList(() => parameters.Excludes);
            app.AddArgument()
                .ValidateWith(new PathValidator(PathType.Directory))
                .AssignTo(() => parameters.BackupDirectory, directory => new DirectoryInfo(directory));
            try
            {
                return app.Run();
            }
            catch (Exception ex)
            {
                return app.HandleError(ex);
            }
            finally
            {
                if (Debugger.IsAttached)
                    ReadLine();
            }
        }

        private static int Handler(object scope)
        {
            var parameters = (Parameters)scope;
            WriteLine($"{parameters.BackupType} backup requested for the directory {parameters.BackupDirectory}");
            if (parameters.Excludes.Count > 0)
            {
                WriteLine(@"Following files to be excluded:");
                foreach (string exclude in parameters.Excludes)
                    WriteLine($"    {exclude}", ConsoleColor.Magenta);
            }
            if (parameters.Verbose)
                WriteLine(@"Verbose output requested.");
            return 0;
        }
    }
}