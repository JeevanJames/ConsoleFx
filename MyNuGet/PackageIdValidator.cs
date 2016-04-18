using ConsoleFx.Parser.Validators;

namespace MyNuGet
{
    public sealed class PackageIdValidator : RegexValidator
    {
        public PackageIdValidator() : base(@"(\w\.?)+")
        {
            Message = "'{0}' is not a valid package ID.";
        }
    }
}