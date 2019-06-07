![ConsoleFx](Logo.png)
# ConsoleFx

[![Build status](https://img.shields.io/appveyor/ci/JeevanJames/consolefx.svg)](https://ci.appveyor.com/project/JeevanJames/consolefx/branch/master) [![Test status](https://img.shields.io/appveyor/tests/JeevanJames/consolefx.svg)](https://ci.appveyor.com/project/JeevanJames/consolefx/branch/master) [![codecov](https://codecov.io/gh/JeevanJames/ConsoleFx/branch/master/graph/badge.svg)](https://codecov.io/gh/JeevanJames/ConsoleFx)

ConsoleFx is a suite of .NET libraries for building command-line (CLI) applications.

It consists of the following packages:

## Packages
ConsoleFx consists of the following NuGet packages. Development packages for continuous integration builds are available on [MyGet](https://myget.org/gallery/consolefx).

Package | Description | NuGet | MyGet
--------|-------------|-------|------
`ConsoleFx.CmdLine.Program` | Write command line programs with sophisticated argument parsing, including error handling, automatic help generation and rich validation support. Supports both Unix and Windows-style arguments. | TBD | TBD
`ConsoleFx.CmdLine.Parser` | Standalone argument parser that is used by `ConsoleFx.CmdLine.Program`. Can be used in non-console program such as Windows Forms, WPF, REPL, etc. to parse command line arguments in a similar fashion. | TBD | TBD
`ConsoleFx.ConsoleExtensions` | Extended console capabilities like color output, prompts, inputting secrets, outputting indented text, progress bars, etc. | TBD | TBD
`ConsoleFx.Prompter` | Rich interactive framework from getting inputs from users. Inspired by the [Inquirer.js](https://github.com/SBoudrias/Inquirer.js) framework for JavaScript. | TBD | TBD
`ConsoleFx.Capture` | Capture of console output from other command-line applications. | TBD | TBD
`ConsoleFx` | Metapackage that contains all major ConsoleFx packages. | [![NuGet Version](http://img.shields.io/nuget/v/ConsoleFx.svg?style=flat)](https://www.nuget.org/packages/ConsoleFx/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ConsoleFx.svg)](https://www.nuget.org/packages/ConsoleFx/) | [![ConsoleFx](https://img.shields.io/myget/consolefx/vpre/ConsoleFx.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx)

The following packages are under development and expected in a future release.

Package | Description | Expected Version
--------|-------------|:---------------:
`ConsoleFx.Art` | Output ASCII art in different styles. | 2.1
`ConsoleFx.UI` | Dynamic creation of Windows Forms and WPF UI to visually input command-line arguments. | TBD

## NuGet packages

|     |     |
|-----|-----|
| `ConsoleFx` | [![NuGet Version](http://img.shields.io/nuget/v/ConsoleFx.svg?style=flat)](https://www.nuget.org/packages/ConsoleFx/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ConsoleFx.svg)](https://www.nuget.org/packages/ConsoleFx/) |


## Development builds
We publish the ongoing development builds from our CI system to our [MyGet repository](https://myget.org/gallery/consolefx).

|     |     |
|-----|-----|
| `ConsoleFx` | [![ConsoleFx](https://img.shields.io/myget/consolefx/vpre/ConsoleFx.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx) |
| `ConsoleFx.Capture` | [![ConsoleFx.Capture](https://img.shields.io/myget/consolefx/v/ConsoleFx.Capture.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.Capture) |
| `ConsoleFx.CmdLine.Abstractions` | [![ConsoleFx.CmdLine.Abstractions](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Abstractions.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Abstractions) |
| `ConsoleFx.CmdLine.Parser` | [![ConsoleFx.CmdLine.Parser](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Parser.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Parser) |
| `ConsoleFx.CmdLine.Program` | [![ConsoleFx.CmdLine.Program](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Program.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Program) |
| `ConsoleFx.ConsoleExtensions` | [![ConsoleFx.ConsoleExtensions](https://img.shields.io/myget/consolefx/v/ConsoleFx.ConsoleExtensions.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.ConsoleExtensions) |
| `ConsoleFx.Prompter` | [![ConsoleFx.Prompter](https://img.shields.io/myget/consolefx/v/ConsoleFx.Prompter.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.Prompter) |
