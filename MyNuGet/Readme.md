## NuGet command-line simulation sample

This sample simulates a large command-line application with multiple sub-commands and help.
The application being simulated is the NuGet CLI.

Documentation for the NuGet CLI can be found at <http://docs.nuget.org/consume/Command-Line-Reference>.

*Note*:

1. None of the NuGet CLI functionalities are implemented. This sample only simulates the command-line argument handling. For all combination of arguments passed, the application will simply dump details of the specified arguments.
2. The only exception is help, which will show proper usage info. However, the help output might differ slightly from the NuGet output in terms of formatting.

### Project structure
Each command (such as install, update, etc.) is defined in a separate sub-folder using a CommandBuilder class.