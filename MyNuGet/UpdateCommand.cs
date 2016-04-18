using System.Collections.Generic;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

namespace MyNuGet
{
    public class UpdateCommand : CommandBuilder
    {
        protected override string Name => "update";

        protected override IEnumerable<Argument> Arguments
        {
            get
            {
                yield return new Argument()
                    .Description("packages.config|solution", "Either the packages.config file or the solution file")
                    .ValidateWith(new FileValidator {
                        ShouldExist = true,
                        AllowedExtensions = {
                            "config",
                            "sln"
                        }
                    });
            }
        }

        protected override IEnumerable<Option> Options
        {
            get
            {
                yield break;
            }
        }
    }
}