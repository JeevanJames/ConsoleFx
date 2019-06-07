![ConsoleFx](Logo.png)
# ConsoleFx

[![Build status](https://img.shields.io/appveyor/ci/JeevanJames/consolefx.svg)](https://ci.appveyor.com/project/JeevanJames/consolefx/branch/master) [![Test status](https://img.shields.io/appveyor/tests/JeevanJames/consolefx.svg)](https://ci.appveyor.com/project/JeevanJames/consolefx/branch/master) [![codecov](https://codecov.io/gh/JeevanJames/ConsoleFx/branch/master/graph/badge.svg)](https://codecov.io/gh/JeevanJames/ConsoleFx)

ConsoleFx is a suite of .NET libraries for building command-line (CLI) applications.

## Build console apps with command line arguments
The following code simulates the following made-up console app:

`COPY <source file> [<destination dir>] [--overwrite] [--create-dir]`

```cs
public class Program : ConsoleProgram
{
    // Declare one property per argument and option.
    
    public string SourceFile { get; set; }
    
    public string DestinationDir { get; set; }
    
    [Option("overwrite")]
    public bool OverwriteExistingFile { get; set; }
    
    [Option("create-dir")]
    public bool CreateDirIfMissing { get; set; }
    
    // Code to execute the console program if all command line args are verified.
    protected int HandleCommand()
    {
        Console.WriteLine($"You want to copy {SourceFile} to {DestinationDir}");
        Console.WriteLine($"Overwrite file if it exists: {OverwriteExistingFile}");
        Console.WriteLine($"Create destination directory if it does not exist: {CreateDirIfMissing}");
        return 0;
    }
    
    // Specify the options and arguments that are accepted by the console app
    protected override IEnumerable<Arg> GetArgs()
    {
        yield return new Argument(nameof(SourceFile))
            .ValidateAsFile(shouldExist: true);
        yield return new Argument(nameof(DestinationDir), optional: true)
            .ValidateAsDirectory();
        yield return new Option("overwrite", "o")
            .UsedAsFlag();
        yield return new Option("create-dir", "c")
            .UsedAsFlag();
    }
    
    public static int Main()
    {
        var program = new Program();
        return program.Run();
    }
}
```

## Packages
ConsoleFx consists of the following NuGet packages. Development packages from continuous integration builds are available on [MyGet](https://myget.org/gallery/consolefx).

Package | Description | Dev Build
--------|-------------|----------
`ConsoleFx.CmdLine.Program` | Write command line programs with sophisticated argument parsing, including error handling, automatic help generation and rich validation support. Supports both Unix and Windows-style arguments. | [![ConsoleFx.CmdLine.Program](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Program.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Program)
`ConsoleFx.CmdLine.Parser` | Standalone argument parser that is used by `ConsoleFx.CmdLine.Program`. Can be used in non-console program such as Windows Forms, WPF, REPL, etc. to parse command line arguments in a similar fashion. | [![ConsoleFx.CmdLine.Parser](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Parser.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Parser)
`ConsoleFx.ConsoleExtensions` | Extended console capabilities like color output, prompts, inputting secrets, outputting indented text, progress bars, etc. | [![ConsoleFx.ConsoleExtensions](https://img.shields.io/myget/consolefx/v/ConsoleFx.ConsoleExtensions.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.ConsoleExtensions)
`ConsoleFx.Prompter` | Rich interactive framework from getting inputs from users. Inspired by the [Inquirer.js](https://github.com/SBoudrias/Inquirer.js) framework for JavaScript. | [![ConsoleFx.Prompter](https://img.shields.io/myget/consolefx/v/ConsoleFx.Prompter.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.Prompter)
`ConsoleFx.Capture` | Capture of console output from other command-line applications. | [![ConsoleFx.Capture](https://img.shields.io/myget/consolefx/v/ConsoleFx.Capture.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.Capture)

### Metapackage
ConsoleFx includes a metapackage that contains all the major packages that would typically needed to build a complex console application.

[![NuGet Version](http://img.shields.io/nuget/v/ConsoleFx.svg?style=flat)](https://www.nuget.org/packages/ConsoleFx/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ConsoleFx.svg)](https://www.nuget.org/packages/ConsoleFx/) [![ConsoleFx](https://img.shields.io/myget/consolefx/vpre/ConsoleFx.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx)

### Under development
The following packages are under development and expected in a future release.

Package | Description | Expected Version
--------|-------------|:---------------:
`ConsoleFx.Art` | Output ASCII art in different styles. | 2.1
`ConsoleFx.UI` | Dynamic creation of Windows Forms and WPF UI to visually input command-line arguments. | TBD
