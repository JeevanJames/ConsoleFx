using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

using static System.Console;
using static ConsoleFx.Utilities.ConsoleEx;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;

namespace TestHarness
{
    public enum BackupType
    {
        Full,
        Incremental,
    }

    internal class Program
    {
        private bool Verbose;
        private BackupType BackupType = BackupType.Full;
        private List<string> Excludes = new List<string>();
        private DirectoryInfo BackupDirectory;

        private static int Main()
        {
            var program = new Program();
            var app = new ConsoleProgram<UnixParserStyle>(program.Handler, scope: program);
            app.AddOption("verbose", "v")
                .Flag(() => program.Verbose);
            app.AddOption("type", "t")
                .ParametersRequired()
                .ValidateWith(new EnumValidator<BackupType>() { Message = "Please specify either Full or Incremental for the backup type." })
                .AssignTo(() => program.BackupType);
            app.AddOption("exclude", "e")
                .Optional(int.MaxValue)
                .ParametersRequired(int.MaxValue)
                .ValidateWith(new RegexValidator(@"^[\w.*?]+$"))
                .AddToList(() => program.Excludes);
            app.AddArgument()
                .ValidateWith(new PathValidator(PathType.Directory))
                .AssignTo(() => program.BackupDirectory, directory => new DirectoryInfo(directory));
            try
            {
                return app.Run();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString(), ConsoleColor.Red);
                ReadLine();
                return -1;
            }
            finally
            {
                if (Debugger.IsAttached)
                    ReadLine();
            }
        }


        private int Handler()
        {
            WriteLine($"{BackupType} backup requested for the directory {BackupDirectory}");
            if (Excludes.Count > 0)
            {
                WriteLine($"Following files to be excluded:");
                foreach (string exclude in Excludes)
                    WriteLine($"    {exclude}");
            }
            if (Verbose)
                WriteLine("Verbose output requested.");
            return 0;
        }
    }
}
