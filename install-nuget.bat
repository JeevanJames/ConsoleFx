@ECHO OFF

REM For a given solution, packages the specified project as a NuGet package
REM and publishes it to NuGet.
REM A .nuspec file with placeholders should already exist in the project directory.
REM
REM Usage: INSTALL-NUGET <project name>
REM
REM   <project name>  The name of the subdirectory where the project exists.

REM Check arguments
IF "%1"=="" GOTO Error_ProjectNotSpecified
SET Project=%1
IF NOT EXIST "%Project%" GOTO Error_ProjectNotFound

REM Move to the project directory.
PUSHD "%Project%"

REM Delete any existing package files in the project directory.
DEL *.nupkg /Q

REM Look for a .csproj file in the directory.
REM If multiple .csproj files found, use the last one.
SET ProjectFile=
FOR %%C IN (*.csproj) DO SET ProjectFile="%%C"
IF NOT EXIST "%ProjectFile%" GOTO Error_MissingProjectFile

REM Build the project in release mode and package it up into a NuGet package.
NUGET PACK "%ProjectFile%" -Build -Properties Configuration=Release

REM Look for a .nupkg file in the directory.
REM If multiple .nupkg files found, use the last one.
SET PackagedFile=
FOR %%P IN (*.nupkg) DO SET PackagedFile="%%P"
IF NOT EXIST "%PackagedFile%" GOTO Error_PackagingFailed

REM Publish the package to NuGet.
NUGET PUSH "%PackagedFile%"

REM Delete the package file.
DEL *.nupkg /Q

REM Return to the solution directory
POPD

GOTO End

:Error_ProjectNotSpecified
ECHO Specify a project to publish to NuGet.
GOTO Usage

:Error_ProjectNotFound
ECHO Specified project '%Project%' was not found.
GOTO Usage

:Error_MissingProjectFile
ECHO Cannot find a project file (.csproj) in the project folder.
GOTO Usage

:Error_PackagingFailed
ECHO It looks like the project could not be packaged correctly.
ECHO Please try again.
GOTO Usage

:Usage
ECHO.
ECHO Usage: %0 (Project)
ECHO.

:End
