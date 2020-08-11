#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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

using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Validators;

namespace CmdLine.Parser.Tests.Commands
{
    [Command("install")]
    public sealed class InstallCommand : Command
    {
        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument()
                .ValidateAnyCondition("Invalid package name",
                    new UriValidator(),
                    new FileValidator(shouldExist: true, allowedExtensions: null));
            yield return new Argument(isOptional: true);

            yield return new Option("save-prod", "p").UsedAsFlag(true);
            yield return new Option("save-dev", "d").UsedAsFlag(true);
            yield return new Option("save-optional", "o").UsedAsFlag(true);
            yield return new Option("no-save").UsedAsFlag(true);

            yield return new Option("save-exact", "e").UsedAsFlag(true);
            yield return new Option("save-bundle", "b").UsedAsFlag(true);
        }
    }
}
