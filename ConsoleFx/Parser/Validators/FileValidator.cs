using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleFx.Parser.Validators
{
    public class FileValidator : Validator<FileInfo>
    {
        public IList<string> AllowedExtensions { get; } = new List<string>();

        public string BaseDirectory { get; set; }

        public bool CheckIfExists { get; set; }

        protected override FileInfo ValidateAsString(string parameterValue)
        {
            try
            {
                return new FileInfo(parameterValue);
            } catch (ArgumentException)
            {
                ValidationFailed(Messages.File_NameInvalid, parameterValue);
            } catch (PathTooLongException)
            {
                ValidationFailed(Messages.File_PathTooLong, parameterValue);
            } catch (NotSupportedException)
            {
                ValidationFailed(Messages.File_NameInvalid, parameterValue);
            }
            throw new NotSupportedException("Should not have reached here.");
        }

        protected override void ValidateAsActualType(FileInfo file, string parameterName)
        {
            if (CheckIfExists && !file.Exists)
                ValidationFailed(Messages.File_Missing, parameterName);

            if (AllowedExtensions != null && AllowedExtensions.Count > 0)
            {
                string extension = file.Extension;
                if (!AllowedExtensions.Any(ext => $".{ext}".Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    StringBuilder allowedExtensions = AllowedExtensions.Aggregate(new StringBuilder(), (sb, ext) => {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        return sb.Append(ext);
                    });
                    ValidationFailed(Messages.File_InvalidExtension, parameterName, allowedExtensions);
                }
            }
        }
    }
}
