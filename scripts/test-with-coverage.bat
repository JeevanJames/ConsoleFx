dotnet test --blame .\test\ConsoleExtensions.Tests\ConsoleExtensions.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
reportgenerator -reports:.\test\ConsoleExtensions.Tests\coverage.opencover.xml -targetdir:.\.coverage\ConsoleExtensions

dotnet test --blame .\test\CmdLine.Abstractions.Tests\CmdLine.Abstractions.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
reportgenerator -reports:.\test\CmdLine.Abstractions.Tests\coverage.opencover.xml -targetdir:.\.coverage\CmdLine.Abstractions

dotnet test --blame .\test\CmdLine.Parser.Tests\CmdLine.Parser.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
reportgenerator -reports:.\test\CmdLine.Parser.Tests\coverage.opencover.xml -targetdir:.\.coverage\CmdLine.Parser
