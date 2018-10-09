#region --- License & Copyright Notice ---
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

using System.Collections.Generic;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;
using ConsoleFx.CmdLineParser.Programs;

namespace MyNuGet.Update
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