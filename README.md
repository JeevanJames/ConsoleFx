# ConsoleFx

|       |     |
|-------|-----|
| **Code**  | [![Build status](https://img.shields.io/appveyor/ci/JeevanJames/consolefx.svg)](https://ci.appveyor.com/project/JeevanJames/consolefx/branch/master) [![Test status](https://img.shields.io/appveyor/tests/JeevanJames/consolefx.svg)](https://ci.appveyor.com/project/JeevanJames/consolefx/branch/master) [![codecov](https://codecov.io/gh/JeevanJames/ConsoleFx/branch/master/graph/badge.svg)](https://codecov.io/gh/JeevanJames/ConsoleFx) |
| **NuGet** | [![NuGet Version](http://img.shields.io/nuget/v/ConsoleFx.svg?style=flat)](https://www.nuget.org/packages/ConsoleFx/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ConsoleFx.svg)](https://www.nuget.org/packages/ConsoleFx/) |

ConsoleFx is a suite of .NET libraries for building command-line (CLI) applications.

It consists of the following packages:

Package | Description
--------|------------
ConsoleFx.CmdLineParser | Sophisticated parsing support for command-line arguments, including error handling and rich validations. Supports both Windows and Unix-style arguments.
ConsoleFx.ConsoleExtensions | Extended console capabilities like color output, secret inputs, outputting indented text, prompts, etc.
ConsoleFx.Art | Outputting ASCII art in different styles
ConsoleFx.Capture | Capturing of console output from external command-line applications
ConsoleFx.Prompter | Rich interactive framework from getting inputs from users. Inspired by the [Inquirer.js](https://github.com/SBoudrias/Inquirer.js) framework for JavaScript.
ConsoleFx.UI | Supports dynamic creation of WinForms and WPF UI to visually input command-line arguments.

## Development builds
We publish the ongoing development builds from our CI system to our [MyGet repository](https://www.myget.org/feed/Packages/consolefx).

|     |     |
|-----|-----|
| ConsoleFx | [![ConsoleFx](https://img.shields.io/myget/consolefx/vpre/ConsoleFx.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx) |
| ConsoleFx.Capture | [![ConsoleFx.Capture](https://img.shields.io/myget/consolefx/v/ConsoleFx.Capture.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.Capture) |
| ConsoleFx.CmdLine.Abstractions | [![ConsoleFx.CmdLine.Abstractions](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Abstractions.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Abstractions) |
| ConsoleFx.CmdLine.Parser | [![ConsoleFx.CmdLine.Parser](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Parser.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Parser) |
| ConsoleFx.CmdLine.Program | [![ConsoleFx.CmdLine.Program](https://img.shields.io/myget/consolefx/v/ConsoleFx.CmdLine.Program.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.CmdLine.Program) |
| ConsoleFx.ConsoleExtensions | [![ConsoleFx.ConsoleExtensions](https://img.shields.io/myget/consolefx/v/ConsoleFx.ConsoleExtensions.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.ConsoleExtensions) |
| ConsoleFx.Prompter | [![ConsoleFx.Prompter](https://img.shields.io/myget/consolefx/v/ConsoleFx.Prompter.svg)](https://www.myget.org/feed/consolefx/package/nuget/ConsoleFx.Prompter) |
