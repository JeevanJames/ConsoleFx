using System;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

namespace MyNuGet
{
    public static class CommonOptions
    {
        public static Option Sources => new Option("Source")
            .Optional(int.MaxValue)
            .ParametersRequired(int.MaxValue)
            .ValidateWith(new CompositeValidator(Validation.PackageSourceInvalid,
                new UriValidator(UriKind.Absolute),
                new FileValidator {
                    ShouldExist = true,
                    AllowedExtensions = {
                        "config"
                    }
                }));
    }
}
