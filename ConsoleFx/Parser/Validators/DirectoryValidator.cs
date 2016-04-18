using System.IO;

namespace ConsoleFx.Parser.Validators
{
    public class DirectoryValidator : Validator<DirectoryInfo>
    {
        public bool ShouldExist { get; set; }

        protected override DirectoryInfo ValidateAsString(string parameterValue)
        {
            throw new System.NotImplementedException();
        }
    }
}