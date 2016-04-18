using System;
using System.Collections.Generic;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

namespace MyNuGet
{
    public sealed class InstallCommand : CommandBuilder
    {
        protected override string Name => "install";

        protected override IEnumerable<Argument> Arguments
        {
            get
            {
                yield return new Argument()
                    .ValidateWith(new CompositeValidator("'{0}' is not a valid package ID or package config file.",
                        new PackageIdValidator(),
                        new PathValidator()));
            }
        }

        protected override IEnumerable<Option> Options
        {
            get
            {
                yield return new Option("Source")
                    .Description(Install.Source)
                    .Optional(int.MaxValue)
                    .ParametersRequired(int.MaxValue)
                    .ValidateWith(new CompositeValidator("Invalid package source '{0}'. Package sources should be either absolute URLs or local directories.",
                        new UriValidator(UriKind.Absolute),
                        new FileValidator {
                            CheckIfExists = true,
                            AllowedExtensions = {
                                "config"
                            }
                        }));

                yield return new Option("OutputDirectory")
                    .Description(Install.OutputDirectory);
                yield return new Option("ExcludeVersion")
                    .Description(Install.ExcludeVersion);
                yield return new Option("Prerelease")
                    .Description(Install.Prerelease);
                yield return new Option("NoCache")
                    .Description(Install.NoCache);
                yield return new Option("RequireConsent")
                    .Description(Install.RequireConsent);
                yield return new Option("SolutionDirectory")
                    .Description(Install.SolutionDirectory);
                yield return new Option("Verbosity")
                    .Description(Install.Verbosity);
                yield return new Option("NonInteractive")
                    .Description(Install.NonInteractive);
                yield return new Option("FileConflictAction")
                    .Description(Install.FileConflictAction);
                yield return new Option("ConfigFile")
                    .Description(Install.ConfigFile);
            }
        }
    }
}