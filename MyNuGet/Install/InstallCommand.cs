using System;
using System.Collections.Generic;
using System.IO;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

namespace MyNuGet.Install
{
    public sealed class InstallCommand : CommandBuilder
    {
        protected override string Name => "install";

        protected override IEnumerable<Argument> Arguments
        {
            get
            {
                yield return new Argument()
                    .Description("package id/config file", "The package ID or path to the packages.config file.")
                    .ValidateWith(new CompositeValidator("'{0}' is not a valid package ID or package config file.",
                        new PackageIdValidator(),
                        new FileValidator()));
            }
        }

        protected override IEnumerable<Option> Options
        {
            get
            {
                yield return CommonOptions.Sources
                    .Description(Install.Source);

                yield return new Option("OutputDirectory")
                    .UsedAsSingleParameter()
                    .ValidateWith(new DirectoryValidator { ShouldExist = true })
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
                    .ValidateWith(new DirectoryValidator { ShouldExist = true })
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
}