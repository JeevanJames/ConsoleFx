#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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
