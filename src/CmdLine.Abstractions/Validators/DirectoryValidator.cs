// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public class DirectoryValidator : Validator
    {
        public DirectoryValidator()
            : base(typeof(DirectoryInfo))
        {
        }

        public DirectoryValidator(bool shouldExist)
            : base(typeof(DirectoryInfo))
        {
            ShouldExist = shouldExist;
        }

        public bool ShouldExist { get; }

        public string InvalidDirectoryNameMessage => Messages.Directory_NameInvalid;

        public string PathTooLongMessage => Messages.Directory_PathTooLong;

        public string DirectoryMissingMessage => Messages.Directory_Missing;

        protected override object ValidateAsString(string parameterValue)
        {
            try
            {
                return new DirectoryInfo(parameterValue);
            }
            catch (ArgumentException)
            {
                ValidationFailed(InvalidDirectoryNameMessage, parameterValue);
            }
            catch (PathTooLongException)
            {
                ValidationFailed(PathTooLongMessage, parameterValue);
            }
            catch (NotSupportedException)
            {
                ValidationFailed(InvalidDirectoryNameMessage, parameterValue);
            }

            throw new NotSupportedException("Should not have reached here.");
        }

        protected override void ValidateAsActualType(object value, string parameterValue)
        {
            var directory = (DirectoryInfo)value;
            if (ShouldExist && !directory.Exists)
                ValidationFailed(DirectoryMissingMessage, parameterValue);
        }
    }

    public static class DirectoryValidatorExtensions
    {
        public static Argument ValidateAsDirectory(this Argument argument, bool shouldExist = false) =>
            argument.ValidateWith(new DirectoryValidator(shouldExist));

        public static Option ValidateAsDirectory(this Option option, bool shouldExist = false,
            int parameterIndex = -1) =>
            option.ValidateWith(parameterIndex, new DirectoryValidator(shouldExist));
    }
}
