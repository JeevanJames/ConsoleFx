using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;
using ConsoleFx.Programs.UsageBuilders;

using static ConsoleFx.Utilities.ConsoleEx;

using static System.Console;

namespace TestHarness
{
    public enum BackupType
    {
        Full,
        Incremental
    }

    public sealed class Program : SingleCommandProgram
    {
        private static int Main()
        {
            var program = new Program { UsageBuilder = new MetadataUsageBuilder() };
            int exitCode = program.Run();
            if (Debugger.IsAttached)
                ReadLine();
            return exitCode;
        }

        public Program() : base(new WindowsParserStyle())
        {
        }

        public bool Verbose { get; set; }
        public BackupType BackupType { get; set; } = BackupType.Full;
        public List<string> Excludes { get; } = new List<string>();
        public DirectoryInfo BackupDirectory { get; set; }
        public FileInfo BackupFile { get; set; }

        protected override IEnumerable<Argument> GetArguments()
        {
            yield return CreateArgument()
                .Description("directory", "The directory to backup. Specified directory must exist.")
                .ValidateWith(new PathValidator(PathType.Directory))
                .AssignTo(() => BackupDirectory, directory => new DirectoryInfo(directory));
            yield return CreateArgument()
                .Description("backup file", "The path to a ZIP file that will contain the backup.")
                .ValidateWith(new PathValidator(checkIfExists: false))
                .ValidateWith(a => {
                    string fullPath = Path.GetFullPath(a);
                    string extension = Path.GetExtension(fullPath);
                    return extension.Equals(".zip", StringComparison.OrdinalIgnoreCase);
                })
                .AssignTo(() => BackupFile, file => new FileInfo(file));
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return CreateOption("verbose", "v")
                .Description(
                    "Displays detailed output in the console. This can be useful for developers, support engineers and administrators.")
                .Flag(() => Verbose);
            yield return CreateOption("type", "t")
                .Description("The type of backup to perform - full or incremental")
                .ParametersRequired()
                .ValidateWith(new EnumValidator<BackupType> {
                    Message = "Please specify either Full or Incremental for the backup type."
                })
                .AssignTo(() => BackupType);
            yield return CreateOption("exclude", "e")
                .Description("File(s) to exclude. Use file name or DOS mask.")
                .Optional(int.MaxValue)
                .ParametersRequired(int.MaxValue)
                .ValidateWith(new RegexValidator(@"^[\w.*?]+$"))
                .AddToList(() => Excludes);
        }

        protected override int Handle()
        {
            WriteLine($"{BackupType} backup requested for the directory {BackupDirectory}");
            if (Excludes.Count > 0)
            {
                WriteLine(@"Following files to be excluded:");
                foreach (string exclude in Excludes)
                    WriteLine($"    {exclude}", ConsoleColor.Magenta);
            }
            if (Verbose)
                WriteLine(@"Verbose output requested.");
            return 0;
        }
    }
}