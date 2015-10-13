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

    internal static class Program
    {
        public static bool Verbose;
        public static BackupType BackupType = BackupType.Full;
        public static List<string> Excludes = new List<string>();
        public static DirectoryInfo BackupDirectory;

        private static int Main()
        {
            var app = new ConsoleProgram<UnixParserStyle>(Handler);
            app.AddOption("verbose", "v")
                .Flag(() => Verbose);
            app.AddOption("type", "t")
                .ParametersRequired()
                .ValidateWith(new EnumValidator<BackupType>() { ErrorMessage = "Please specify either Full or Incremental for the backup type." })
                .AssignTo(() => BackupType);
            app.AddOption("exclude", "e")
                .Optional(int.MaxValue)
                .ParametersRequired(int.MaxValue)
                .ValidateWith(new RegexValidator(@"^[\w.*?]+$"))
                .AddToList(() => Excludes);
            app.AddArgument()
                .ValidateWith(new PathValidator(PathType.Directory))
                .AssignTo(() => BackupDirectory, directory => new DirectoryInfo(directory));
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


        private static int Handler()
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
