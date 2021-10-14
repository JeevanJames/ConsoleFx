# ((Get-Content -path ./metapackage/ConsoleFx.nuspec -Raw) -replace '0.1.0',$env:APPVEYOR_BUILD_VERSION) | Set-Content -Path ./metapackage/ConsoleFx.nuspec
# nuget pack ./metapackage/ConsoleFx.nuspec -OutputDirectory ./metapackage -OutputFileNamesWithoutVersion
# dotnet nuget push ./metapackage/ConsoleFx.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY

Write-Host "****** CmdLine.Abstractions ******"
dotnet pack ./src/CmdLine.Abstractions/CmdLine.Abstractions.csproj --include-symbols --include-source -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
dotnet nuget push ./src/CmdLine.Abstractions/bin/Release/ConsoleFx.CmdLine.Abstractions.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY -ss $env:MYGET_SYMBOLS_FEED -sk $env:MYGET_SYMBOLS_API_KEY

# dotnet pack ./src/CmdLine.Abstractions/CmdLine.Abstractions.Sources.csproj -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
# dotnet nuget push ./src/CmdLine.Abstractions/bin/Release/ConsoleFx.CmdLine.Abstractions.Sources.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY

Write-Host "****** CmdLine.Parser ******"
dotnet pack ./src/CmdLine.Parser/CmdLine.Parser.csproj --include-symbols --include-source -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
dotnet nuget push ./src/CmdLine.Parser/bin/Release/ConsoleFx.CmdLine.Parser.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY -ss $env:MYGET_SYMBOLS_FEED -sk $env:MYGET_SYMBOLS_API_KEY

# dotnet pack ./src/CmdLine.Parser/CmdLine.Parser.Sources.csproj -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
# dotnet nuget push ./src/CmdLine.Parser/bin/Release/ConsoleFx.CmdLine.Parser.Sources.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY

Write-Host "****** CmdLine.Program ******"
dotnet pack ./src/CmdLine.Program/CmdLine.Program.csproj --include-symbols --include-source -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
dotnet nuget push ./src/CmdLine.Program/bin/Release/ConsoleFx.CmdLine.Program.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY -ss $env:MYGET_SYMBOLS_FEED -sk $env:MYGET_SYMBOLS_API_KEY

# dotnet pack ./src/CmdLine.Program/CmdLine.Program.Sources.csproj -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
# dotnet nuget push ./src/CmdLine.Program/bin/Release/ConsoleFx.CmdLine.Program.Sources.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY

Write-Host "****** ConsoleExtensions ******"
dotnet pack ./src/ConsoleExtensions/ConsoleExtensions.csproj --include-symbols -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
dotnet nuget push ./src/ConsoleExtensions/bin/Release/ConsoleFx.ConsoleExtensions.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY -ss $env:MYGET_SYMBOLS_FEED -sk $env:MYGET_SYMBOLS_API_KEY

Write-Host "****** Prompter ******"
dotnet pack ./src/Prompter/Prompter.csproj --include-symbols --include-source -c Release /p:Version=$env:APPVEYOR_BUILD_VERSION
dotnet nuget push ./src/Prompter/bin/Release/ConsoleFx.Prompter.$env:APPVEYOR_BUILD_VERSION.nupkg -s $env:MYGET_FEED -k $env:MYGET_API_KEY -ss $env:MYGET_SYMBOLS_FEED -sk $env:MYGET_SYMBOLS_API_KEY
