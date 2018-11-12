﻿#region --- License & Copyright Notice ---
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleFx.CmdLineParser.Validators
{
    public class FileValidator : Validator<FileInfo>
    {
        public FileValidator()
        {
            AllowedExtensions = new List<string>();
        }

        public FileValidator(params string[] allowedExtensions) : this(false, allowedExtensions)
        {
        }

        public FileValidator(bool shouldExist, IEnumerable<string> allowedExtensions)
        {
            if (allowedExtensions == null)
                throw new ArgumentNullException(nameof(allowedExtensions));
            ShouldExist = shouldExist;
            AllowedExtensions = new List<string>(allowedExtensions);
        }

        public IList<string> AllowedExtensions { get; }

        //TODO: Implement
        public string BaseDirectory { get; }

        public bool ShouldExist { get; set; }

        public string InvalidFileNameMessage => Messages.File_NameInvalid;
        public string PathTooLongMessage => Messages.File_PathTooLong;
        public string FileMissingMessage => Messages.File_Missing;
        public string InvalidExtensionMessage => Messages.File_InvalidExtension;

        protected override FileInfo ValidateAsString(string parameterValue)
        {
            try
            {
                return new FileInfo(parameterValue);
            }
            catch (ArgumentException)
            {
                ValidationFailed(InvalidFileNameMessage, parameterValue);
            }
            catch (PathTooLongException)
            {
                ValidationFailed(PathTooLongMessage, parameterValue);
            }
            catch (NotSupportedException)
            {
                ValidationFailed(InvalidFileNameMessage, parameterValue);
            }

            throw new NotSupportedException("Should not have reached here.");
        }

        protected override void ValidateAsActualType(FileInfo file, string parameterName)
        {
            if (ShouldExist && !file.Exists)
                ValidationFailed(FileMissingMessage, parameterName);

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
                    ValidationFailed(InvalidExtensionMessage, parameterName, allowedExtensions);
                }
            }
        }
    }

    public static class FileValidatorExtensions
    {
        public static Argument ValidateAsFile(this Argument argument, bool shouldExist,
            IEnumerable<string> allowedExtensions) =>
            argument.ValidateWith(new FileValidator(shouldExist, allowedExtensions));

        public static Option ValidateAsFile(this Option option, bool shouldExist, IEnumerable<string> allowedExtensions,
            int parameterIndex = -1) =>
            option.ValidateWith(parameterIndex, new FileValidator(shouldExist, allowedExtensions));
    }
}
