#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

namespace ConsoleFx.Parser.Validators
{
    public sealed class PathValidator : SingleMessageValidator
    {
        public PathValidator(PathType pathType = PathType.File, bool checkIfExists = true)
            : base(Messages.Path)
        {
            PathType = pathType;
            CheckIfExists = checkIfExists;
        }

        public override void Validate(string parameterValue)
        {
            if (!CheckIfExists)
                return;

            Func<string, bool> checker = PathType == PathType.File ? File.Exists : new Func<string, bool>(Directory.Exists);
            if (!checker(parameterValue))
                ValidationFailed(parameterValue);
        }

        public bool CheckIfExists { get; set; }

        public PathType PathType { get; set; }
    }

    public enum PathType
    {
        File,
        Directory
    }
}