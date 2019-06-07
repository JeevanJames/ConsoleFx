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

namespace ConsoleFx.CmdLine.Program
{
    public static class HelpExtensions
    {
        public static Argument Description(this Argument argument, string description, string name = null)
        {
            argument["Description"] = description;
            if (!string.IsNullOrWhiteSpace(name))
                argument["Name"] = name;
            return argument;
        }

        public static Command Description(this Command command, string description, string name = null)
        {
            command["Description"] = description;
            if (!string.IsNullOrWhiteSpace(name))
                command["Name"] = name;
            return command;
        }

        public static Option Description(this Option option, string description)
        {
            option["Description"] = description;
            return option;
        }

        public static Command Grouping(this Command command, string groupName, string description)
        {
            command["GroupName"] = groupName;
            command["GroupDescription"] = description;
            return command;
        }

        public static TArg Order<TArg>(this TArg arg, int order)
            where TArg : Arg
        {
            arg["Order"] = order.ToString();
            return arg;
        }
    }
}
