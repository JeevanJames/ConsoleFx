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

using ConsoleFx.CmdLine.Parser;
using ConsoleFx.CmdLine.Program.ErrorHandlers;
using ConsoleFx.CmdLine.Program.HelpBuilders;

using ParserStyle = ConsoleFx.CmdLine.Parser.Style;

namespace ConsoleFx.CmdLine.Program
{
    /// <summary>
    ///     Represents a console program.
    /// </summary>
    public class ConsoleProgram : Command
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ParserStyle.ArgStyle _argStyle;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private HelpBuilder _helpBuilder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleProgram"/> class.
        /// </summary>
        public ConsoleProgram()
        {
            ProgramAttribute programAttribute = GetType().GetCustomAttribute<ProgramAttribute>(true);
            if (programAttribute is null)
            {
                AddName(Assembly.GetEntryAssembly().GetName().Name);
                _argStyle = new ParserStyle.UnixArgStyle();
                Grouping = ArgGrouping.DoesNotMatter;
            }
            else
            {
                string name = string.IsNullOrWhiteSpace(programAttribute.Name)
                    ? Assembly.GetEntryAssembly().GetName().Name
                    : programAttribute.Name;
                AddName(name);
                _argStyle = CreateArgStyle(programAttribute.Style);
                Grouping = programAttribute.Grouping;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleProgram"/> class with the specified
        ///     name, arg style and grouping.
        /// </summary>
        /// <param name="name">The name of the program.</param>
        /// <param name="argStyle">The expected argument style.</param>
        /// <param name="grouping">The expected arg grouping.</param>
        public ConsoleProgram(string name, ArgStyle argStyle, ArgGrouping grouping = ArgGrouping.DoesNotMatter)
            : base(caseSensitive: false, name)
        {
            _argStyle = CreateArgStyle(argStyle);
            Grouping = grouping;
        }

        /// <summary>
        ///     Gets the expected grouping of the args.
        /// </summary>
        public ArgGrouping Grouping { get; }

        /// <summary>
        ///     Gets or sets the <see cref="HelpBuilder"/> to use to display the help.
        /// </summary>
        public HelpBuilder HelpBuilder
        {
            get => _helpBuilder ?? (_helpBuilder = new DefaultHelpBuilder(_argStyle.GetDefaultHelpOptionNames().ToArray()));
            set => _helpBuilder = value;
        }

        /// <summary>
        ///     Gets or sets the <see cref="ErrorHandler"/> to use to handle any exceptions thrown
        ///     when parsing and executing the application.
        /// </summary>
        public ErrorHandler ErrorHandler { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to display help when an error occurs when
        ///     parsing and executing the application.
        /// </summary>
        public bool DisplayHelpOnError { get; set; }

        public bool VerifyHelp { get; set; }

        public override void DisplayHelp(Command command = null)
        {
            HelpBuilder helpBuilder = HelpBuilder;
            if (helpBuilder != null)
                helpBuilder.DisplayHelp(command ?? this);
        }

        /// <summary>
        ///     Runs the console program after parsing the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">The args to parse.</param>
        /// <returns>
        ///     The numeric code that represents the result of the console program execution.
        /// </returns>
        public int Run(IEnumerable<string> args = null)
        {
            if (args is null)
                args = new string[0];

            ParseResult parseResult = null;
            IReadOnlyList<PrePostHandlerAttribute> attributes = null;

            var parser = new Parser.Parser(this, _argStyle, Grouping);
            try
            {
                // Parse the args and assign to the properties in the resultant command.
                parseResult = parser.Parse(args);

                // Assign the properties on the command object from the parse result.
                AssignArgumentProperties(parseResult, parseResult.Command.Arguments);
                AssignOptionProperties(parseResult, parseResult.Command.Options);

                // Check if the help option is specified. If it is, display the help and get out.
                HelpBuilder helpBuilder = HelpBuilder;
                if (VerifyHelp)
                    helpBuilder.VerifyHelp(parseResult.Command);
                if (parseResult.TryGetOption(helpBuilder.Name, out bool displayHelp) && displayHelp)
                {
                    helpBuilder.DisplayHelp(parseResult.Command);
                    return 0;
                }

                // Get any pre post attributes applied on the command.
                // These attributes will run custom code before and after the command handler and also
                // when an exception is thrown.
                attributes = parseResult.Command.GetType()
                    .GetCustomAttributes<PrePostHandlerAttribute>(true)
                    .ToList();

                // Run all pre-handler attributes.
                foreach (PrePostHandlerAttribute attribute in attributes)
                    attribute.BeforeHandler(parseResult.Command);

                // Execute the command handler.
                return parseResult.Command.Handler(parseResult);
            }
            catch (Exception ex)
            {
                // Run the exception through all attribute error handlers.
                int? attributeErrorCode = null;
                if (attributes != null)
                {
                    foreach (PrePostHandlerAttribute attribute in attributes)
                        attributeErrorCode = attribute.OnException(ex, parseResult?.Command);
                }

                // Run the configured error handler.
                int errorCode = (ErrorHandler ?? new DefaultErrorHandler()).HandleError(ex);
                DebugOutput.Write(ex);

                // Display help if so configured.
                if (DisplayHelpOnError)
                    HelpBuilder.DisplayHelp(parseResult?.Command ?? this);

                // If the attributes have returned an error code, use that, otherwise use the error
                // code returned from the error handler.
                return attributeErrorCode.GetValueOrDefault(errorCode);
            }
            finally
            {
                // Run all post-handler attributes.
                if (attributes != null)
                {
                    foreach (PrePostHandlerAttribute attribute in attributes)
                        attribute.AfterHandler(parseResult?.Command);
                }
            }
        }

        /// <summary>
        ///     Runs the console program after parsing the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">The args to parse.</param>
        /// <returns>
        ///     The numeric code that represents the result of the console program execution.
        /// </returns>
        public int Run(params string[] args)
        {
            return Run((IEnumerable<string>)args);
        }

        /// <summary>
        ///     Runs the console program after parsing the command line args.
        /// </summary>
        /// <returns>
        ///     The numeric code that represents the result of the console program execution.
        /// </returns>
        public int RunWithCommandLineArgs()
        {
            return Run(Environment.GetCommandLineArgs().Skip(1));
        }

        /// <summary>
        ///     Add help options to the command as universal options. These options will be added to
        ///     all commands.
        /// </summary>
        /// <returns>The <see cref="Option"/> that represents help.</returns>
        protected sealed override IEnumerable<Option> GetUniversalOptions()
        {
            HelpBuilder helpBuilder = HelpBuilder;

            var option = new Option(helpBuilder.AllNames.ToArray())
                .UsedAsFlag()
                .UnderGroups(int.MinValue)
                .HideHelp();
            yield return option;
        }

        private static ParserStyle.ArgStyle CreateArgStyle(ArgStyle argStyle)
        {
            switch (argStyle)
            {
                case ArgStyle.Unix:
                    return new ParserStyle.UnixArgStyle();
                case ArgStyle.Windows:
                    return new ParserStyle.WindowsArgStyle();
            }

            throw new NotSupportedException($"Unsupported argument style: '{argStyle}'.");
        }

        private static void AssignOptionProperties(ParseResultBase parseResult, Options args)
        {
            Type type = parseResult.Command.GetType();

            foreach (Option arg in args)
            {
                if (arg.AssignedPropertyName is null && arg.AssignedProperty is null)
                    continue;

                if (arg.AssignedProperty is null)
                {
                    PropertyInfo property = type.GetProperty(arg.AssignedPropertyName);
                    if (property is null)
                        throw new ParserException(-1, $"Cannot find a property named {arg.AssignedPropertyName} to be assigned to the {arg.Name} arg.");
                    arg.AssignedProperty = property;
                }

                if (parseResult.TryGetOption(arg.Name, out object value))
                    arg.AssignedProperty.SetValue(parseResult.Command, value);
            }
        }

        private static void AssignArgumentProperties(ParseResultBase parseResult, Arguments arguments)
        {
            Type type = parseResult.Command.GetType();

            for (int i = 0; i < arguments.Count; i++)
            {
                Argument argument = arguments[i];

                if (argument.AssignedPropertyName is null && argument.AssignedProperty is null)
                    continue;

                if (argument.AssignedProperty is null)
                {
                    PropertyInfo property = type.GetProperty(argument.AssignedPropertyName);
                    if (property is null)
                        throw new ParserException(-1, $"Cannot find a property named {argument.AssignedPropertyName} to be assigned to the {argument.Order} argument.");
                    argument.AssignedProperty = property;
                }

                if (parseResult.TryGetArgument(i, out object value))
                    argument.AssignedProperty.SetValue(parseResult.Command, value);
            }
        }
    }
}
