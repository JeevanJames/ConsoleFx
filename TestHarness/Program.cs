using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using static ConsoleFx.Utilities.ConsoleEx;
using static System.Console;

namespace TestHarness
{
    public enum BackupType
    {
        Full,
        Incremental,
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
        private static readonly Parameters _parameters = new Parameters();

        private static int Main()
        {
            var app = new ConsoleProgram<WindowsParserStyle>(Handler, _parameters);
            app.AddOption("verbose", "v")
                .Flag(() => _parameters.Verbose);
            app.AddOption("type", "t")
                .ParametersRequired()
                .ValidateWith(new EnumValidator<BackupType> { Message = "Please specify either Full or Incremental for the backup type." })
                .AssignTo(() => _parameters.BackupType);
            app.AddOption("exclude", "e")
                .Optional(int.MaxValue)
                .ParametersRequired(int.MaxValue)
                .ValidateWith(new RegexValidator(@"^[\w.*?]+$"))
                .AddToList(() => _parameters.Excludes);
            app.AddArgument()
                .ValidateWith(new PathValidator(PathType.Directory))
                .AssignTo(() => _parameters.BackupDirectory, directory => new DirectoryInfo(directory));
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


        private static int Handler()
        {
            WriteLine($"{_parameters.BackupType} backup requested for the directory {_parameters.BackupDirectory}");
            if (_parameters.Excludes.Count > 0)
            {
                WriteLine(@"Following files to be excluded:");
                foreach (string exclude in _parameters.Excludes)
                    WriteLine($"    {exclude}", ConsoleColor.DarkMagenta);
            }
            if (_parameters.Verbose)
                WriteLine(@"Verbose output requested.");
            return 0;
        }
    }
}
