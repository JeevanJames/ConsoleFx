// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public class FileValidator : Validator
    {
        public FileValidator(params string[] allowedExtensions)
            : this(false, allowedExtensions)
        {
        }

        public FileValidator(bool shouldExist = false, IEnumerable<string> allowedExtensions = null)
            : base(typeof(FileInfo))
        {
            ShouldExist = shouldExist;
            AllowedExtensions = allowedExtensions is not null ? new List<string>(allowedExtensions) : new List<string>();
        }

        public IList<string> AllowedExtensions { get; }

        //TODO: Implement
        public string BaseDirectory { get; }

        public bool ShouldExist { get; set; }

        public string InvalidFileNameMessage => Messages.File_NameInvalid;

        public string PathTooLongMessage => Messages.File_PathTooLong;

        public string FileMissingMessage => Messages.File_Missing;

        public string InvalidExtensionMessage => Messages.File_InvalidExtension;

        protected override object ValidateAsString(string parameterValue)
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

        protected override void ValidateAsActualType(object value, string parameterValue)
        {
            var file = (FileInfo)value;

            if (ShouldExist && !file.Exists)
                ValidationFailed(FileMissingMessage, parameterValue);

            if (AllowedExtensions is not null && AllowedExtensions.Count > 0)
            {
                string extension = file.Extension;
                if (!AllowedExtensions.Any(ext => $".{ext}".Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    StringBuilder allowedExtensions = AllowedExtensions.Aggregate(new StringBuilder(), (sb, ext) =>
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        return sb.Append(ext);
                    });
                    ValidationFailed(InvalidExtensionMessage, parameterValue, allowedExtensions);
                }
            }
        }
    }

    public static class FileValidatorExtensions
    {
        public static Argument ValidateAsFile(this Argument argument, bool shouldExist) =>
            argument.ValidateWith(new FileValidator(shouldExist));

        public static Argument ValidateAsFile(this Argument argument, bool shouldExist,
            IEnumerable<string> allowedExtensions) =>
            argument.ValidateWith(new FileValidator(shouldExist, allowedExtensions));

        public static Argument ValidateAsFile(this Argument argument, bool shouldExist,
            params string[] allowedExtensions) =>
            argument.ValidateWith(new FileValidator(shouldExist, allowedExtensions));

        public static Option ValidateAsFile(this Option option, bool shouldExist, int parameterIndex = -1) =>
            option.ValidateWith(parameterIndex, new FileValidator(shouldExist));

        public static Option ValidateAsFile(this Option option, bool shouldExist, IEnumerable<string> allowedExtensions,
            int parameterIndex = -1) =>
            option.ValidateWith(parameterIndex, new FileValidator(shouldExist, allowedExtensions));
    }
}
