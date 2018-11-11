#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Collections.Generic;
using System.IO;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Programs;
using ConsoleFx.CmdLineParser.Validators;

namespace MyNuGet.Install
{
    public sealed class InstallCommand : CommandBuilder
    {
        protected override string Name => "install";

        protected override IEnumerable<Argument> GetArguments()
        {
            yield return new Argument()
                .Description("package id/config file", "The package ID or path to the packages.config file.")
                .ValidateWith(new CompositeValidator("'{0}' is not a valid package ID or package config file.",
                    new PackageIdValidator(),
                    new FileValidator()));
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return CommonOptions.Sources
                .Description(Install.Source);

            yield return new Option("OutputDirectory")
                .UsedAsSingleParameter()
                .ValidateWith(new DirectoryValidator {ShouldExist = true})
                .ParamsOfType<DirectoryInfo>()
                .Description(Install.OutputDirectory);
            yield return new Option("ExcludeVersion")
                .UsedAsFlag()
                .Description(Install.ExcludeVersion);

            yield return new Option("Prerelease")
                .UsedAsFlag()
                .Description(Install.Prerelease);

            yield return new Option("NoCache")
                .UsedAsFlag()
                .Description(Install.NoCache);

            yield return new Option("RequireConsent")
                .UsedAsFlag()
                .Description(Install.RequireConsent);

            yield return new Option("SolutionDirectory")
                .UsedAsSingleParameter()
                .ValidateWith(new DirectoryValidator {ShouldExist = true})
                .ParamsOfType<DirectoryInfo>()
                .Description(Install.SolutionDirectory);

            yield return new Option("Verbosity")
                .UsedAsSingleParameter()
                .ValidateWith(new EnumValidator<Verbosity>())
                .Description(Install.Verbosity);

            yield return new Option("NonInteractive")
                .UsedAsFlag()
                .Description(Install.NonInteractive);

            yield return new Option("FileConflictAction")
                .UsedAsSingleParameter()
                .ValidateWith(new EnumValidator<FileConflictAction>())
                .Description(Install.FileConflictAction);
        }
    }
}