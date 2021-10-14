dotnet test --blame ./test/CmdLine.Abstractions.Tests/CmdLine.Abstractions.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
dotnet test --blame ./test/CmdLine.Parser.Tests/CmdLine.Parser.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
dotnet test --blame ./test/ConsoleExtensions.Tests/ConsoleExtensions.Tests.csproj /p:CollectCoverage=true /p:Exclude=\"[xunit.*]*\" /p:CoverletOutputFormat=opencover
