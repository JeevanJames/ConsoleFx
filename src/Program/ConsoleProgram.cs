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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineParser;

using ParserStyle = ConsoleFx.CmdLineParser.Style;

namespace ConsoleFx.Program
{
    /// <summary>
    ///     Represents a console program.
    /// </summary>
    public class ConsoleProgram : ProgramCommand
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ArgStyle _argStyle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleProgram"/> class with the specified
        ///     arg style and grouping.
        /// </summary>
        /// <param name="argStyle">The expected argument style.</param>
        /// <param name="grouping">The expected arg grouping.</param>
        public ConsoleProgram(ArgStyle argStyle, ArgGrouping grouping = ArgGrouping.DoesNotMatter)
        {
            _argStyle = argStyle;
            Grouping = grouping;
        }

        /// <summary>
        ///     Gets the expected grouping of the args.
        /// </summary>
        public ArgGrouping Grouping { get; }

        /// <summary>
        ///     Runs the console program after parsing the command-line args.
        /// </summary>
        /// <param name="args">The args to parse.</param>
        /// <returns>The numeric code that represents the result of the console program execution.</returns>
        public int Run(IEnumerable<string> args = null)
        {
            if (args is null)
                args = Environment.GetCommandLineArgs().Skip(1);

            var parser = new Parser(this, CreateArgStyle(), Grouping);
            try
            {
                ParseResult parseResult = parser.Parse(args);

                var programCommand = (ProgramCommand)parseResult.Command;
                programCommand.ParseResult = parseResult;
                AssignProperties(programCommand);
                return programCommand.Handler(ParseResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }

        private ParserStyle.ArgStyle CreateArgStyle()
        {
            switch (_argStyle)
            {
                case ArgStyle.Unix:
                    return new ParserStyle.UnixArgStyle();
                case ArgStyle.Windows:
                    return new ParserStyle.WindowsArgStyle();
            }

            throw new NotSupportedException($"Unsupported argument style: '{_argStyle}'.");
        }

        private void AssignProperties(ProgramCommand command)
        {
            Type type = command.GetType();

            // Get all potentially-assignable properties.
            // i.e. all public instance properties that are read/write.
            List<PropertyInfo> properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.CanRead && pi.CanWrite)
                .ToList();

            foreach (PropertyInfo property in properties)
            {
                string argName;
                bool hasValue;
                object value;

                Attribute attribute = Attribute.GetCustomAttribute(property, typeof(ArgAttribute), true);
                if (attribute != null)
                {
                    argName = ((ArgAttribute)attribute).Name;
                    hasValue = attribute is OptionAttribute
                        ? command.ParseResult.TryGetOption(argName, out value)
                        : command.ParseResult.TryGetArgument(argName, out value);
                }
                else
                {
                    argName = property.Name;
                    hasValue = command.ParseResult.TryGetOption(argName, out value);
                    if (!hasValue)
                        hasValue = command.ParseResult.TryGetArgument(argName, out value);
                }

                // Only throw an exception is there is no arg found for a property with an Option
                // or Argument attribute, as they have been explicitly marked as args.
                if (!hasValue)
                {
                    if (attribute != null)
                        throw new InvalidOperationException($"Cannot find an arg named '{argName}' to assign to the {property.Name} property.");
                    continue;
                }

                property.SetValue(command, value);
            }
        }
    }
}
