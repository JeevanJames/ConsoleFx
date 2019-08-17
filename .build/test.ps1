dotnet test --blame ./test/CmdLine.Parser.Tests/CmdLine.Parser.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
codecov -f "./test/CmdLine.Parser.Tests/coverage.opencover.xml"

dotnet test --blame ./test/ConsoleExtensions.Tests/ConsoleExtensions.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
codecov -f "./test/ConsoleExtensions.Tests/coverage.opencover.xml"
