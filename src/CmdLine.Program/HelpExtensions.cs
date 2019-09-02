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

namespace ConsoleFx.CmdLine.Program
{
    public static class HelpExtensions
    {
        public static Argument Help(this Argument argument, string description, string name = null)
        {
            if (description is null)
                throw new ArgumentNullException(nameof(description));

            var metadata = (IMetadataObject)argument;
            metadata[Keys.Description] = description;
            if (!string.IsNullOrWhiteSpace(name))
                metadata[Keys.Name] = name;
            return argument;
        }

        public static Command Help(this Command command, string description, string name = null)
        {
            if (description is null)
                throw new ArgumentNullException(nameof(description));

            var metadata = (IMetadataObject)command;
            metadata[Keys.Description] = description;
            if (!string.IsNullOrWhiteSpace(name))
                metadata[Keys.Name] = name;
            return command;
        }

        public static Option Help(this Option option, string description)
        {
            if (description is null)
                throw new ArgumentNullException(nameof(description));

            var metadata = (IMetadataObject)option;
            metadata[Keys.Description] = description;
            return option;
        }

        public static TArg Category<TArg>(this TArg arg, string name, string description)
            where TArg : Arg, IMetadataObject
        {
            arg[Keys.CategoryName] = name;
            arg[Keys.CategoryDescription] = description;
            return arg;
        }

        public static TArg Order<TArg>(this TArg arg, int order)
            where TArg : Arg, IMetadataObject
        {
            arg[Keys.Order] = order.ToString();
            return arg;
        }

        public static TArg HideHelp<TArg>(this TArg arg)
            where TArg : Arg, IMetadataObject
        {
            arg[Keys.Hide] = true;
            return arg;
        }

        public static class Keys
        {
            public const string Name = "Name";
            public const string Description = "Description";

            public const string CategoryName = "CategoryName";
            public const string CategoryDescription = "CategoryDescription";

            public const string Order = "Order";

            public const string Hide = "HideHelp";
        }
    }
}
