// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ConsoleFx.CmdLine.Internals;
using ConsoleFx.CmdLine.Program.ErrorHandlers;
using ConsoleFx.CmdLine.Program.HelpBuilders;
using ConsoleFx.CmdLine.Validators;

using ParserStyle = ConsoleFx.CmdLine.Parser.Style;

namespace ConsoleFx.CmdLine.Program
{
    /// <summary>
    ///     Represents a console program.
    /// </summary>
    public class ConsoleProgram : Command
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ParserStyle.ArgStyle _argStyle;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ArgGrouping _grouping;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private HelpBuilder _helpBuilder;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ErrorHandler _errorHandler;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _displayHelpOnError;

        /// <summary>
        ///     This method is called using reflection from the base <see cref="Command"/> class.
        ///     See the <see cref="Command"/> constructor for more information.
        /// </summary>
        protected new void ProcessAttribute()
        {
            ProgramAttribute programAttribute = GetType().GetCustomAttribute<ProgramAttribute>(true);
            if (programAttribute is null)
            {
                // Name defaults to the executable file name
                AddName(Assembly.GetEntryAssembly()?.GetName().Name ?? "program");

                // Argument style defaults to Unix
                _argStyle = new ParserStyle.UnixArgStyle();

                // Grouping defaults to DoesNotMatter
                _grouping = ArgGrouping.DoesNotMatter;
            }
            else
            {
                string name = programAttribute.Name ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "program";
                AddName(name);
                _argStyle = CreateArgStyle(programAttribute.Style);
                _grouping = programAttribute.Grouping;
            }
        }

        /// <summary>
        ///     Gets the expected grouping of the args.
        /// </summary>
        public ArgGrouping Grouping => _grouping;

        /// <summary>
        ///     Gets the <see cref="HelpBuilder"/> to use to display the help.
        /// </summary>
        public HelpBuilder HelpBuilder
        {
            get
            {
                if (_helpBuilder is null)
                {
                    string[] optionNames = _argStyle.GetDefaultHelpOptionNames().ToArray();
                    _helpBuilder = new DefaultHelpBuilder(optionNames);
                }

                return _helpBuilder;
            }
        }

        public ConsoleProgram WithHelpBuilder<THelpBuilder>()
            where THelpBuilder : HelpBuilder, new()
        {
            _helpBuilder = new THelpBuilder();
            return this;
        }

        public ConsoleProgram WithHelpBuilder<THelpBuilder>(Func<THelpBuilder> helpBuilderFactory)
            where THelpBuilder : HelpBuilder
        {
            if (helpBuilderFactory is null)
                throw new ArgumentNullException(nameof(helpBuilderFactory));
            _helpBuilder = helpBuilderFactory();
            return this;
        }

        /// <summary>
        ///     Gets the <see cref="ErrorHandler"/> to use to handle any exceptions thrown when parsing
        ///     and executing the application.
        /// </summary>
        public ErrorHandler ErrorHandler => _errorHandler ??= new DefaultErrorHandler();

        public ConsoleProgram HandleErrorsWith(Func<Exception, int> errorHandler)
        {
            if (errorHandler is null)
                throw new ArgumentNullException(nameof(errorHandler));
            _errorHandler = new DelegateErrorHandler(errorHandler);
            return this;
        }

        public ConsoleProgram HandleErrorsWith<TErrorHandler>()
            where TErrorHandler : ErrorHandler, new()
        {
            _errorHandler = new TErrorHandler();
            return this;
        }

        public ConsoleProgram HandleErrorsWith<TErrorHandler>(Func<TErrorHandler> errorHandlerFactory)
            where TErrorHandler : ErrorHandler
        {
            if (errorHandlerFactory is null)
                throw new ArgumentNullException(nameof(errorHandlerFactory));
            _errorHandler = errorHandlerFactory();
            return this;
        }

        /// <summary>
        ///     Gets a value indicating whether to display help when an error occurs when parsing and
        ///     executing the application.
        /// </summary>
        public ConsoleProgram DisplayHelpOnError()
        {
            _displayHelpOnError = true;
            return this;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the help builder should verify that all args
        ///     have the necessary information to display the help. Help information is stored in the
        ///     metadata, and each help builder can use different metadata to display the help.
        ///     <para/>
        ///     When this property is set to <c>true</c>, the <see cref="Program.HelpBuilder.VerifyHelp(Command)"/>
        ///     method is called and if any necessary information is missing, an exception is thrown.
        /// </summary>
        /// <remarks>
        ///     It is recommended to set this value to true in debug mode to ensure that no needed help
        ///     information is unspecified. It is not recommended to use in release mode.
        /// </remarks>
        public bool VerifyHelp { get; set; }

        /// <summary>
        ///     Displays the help. By default, this method calls the assigned
        ///     <see cref="ConsoleProgram.HelpBuilder"/> to display the help, but it can be derived to
        ///     display help in a customized manner.
        ///     <para/>
        ///     One common scenario for overriding this method is to display custom banners or footers
        ///     along with the help.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to display help for.</param>
        public override void DisplayHelp(Command command = null)
        {
            HelpBuilder?.DisplayHelp(command ?? this);
        }

        /// <summary>
        ///     Runs the console program after parsing the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">The args to parse.</param>
        /// <returns>
        ///     The numeric code that represents the result of the console program execution.
        /// </returns>
        public async Task<int> RunAsync(IEnumerable<string> args = null)
        {
            args ??= Array.Empty<string>();

            IParseResult parseResult = null;
            IReadOnlyList<PrePostHandlerAttribute> prepostHandlers = null;

            var parser = new Parser.Parser(this, _argStyle, Grouping);
            try
            {
                // Parse the args and assign to the properties in the resultant command.
                parseResult = parser.Parse(args);

                // Assign the properties on the command object from the parse result.
                AssignArgumentProperties(parseResult, parseResult.Command.Arguments);
                AssignOptionProperties(parseResult, parseResult.Command.Options);

                string validationError = parseResult.Command.Validate();
                if (validationError is not null)
                    throw new ValidationException(validationError, null, null);

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
                prepostHandlers = parseResult.Command.GetType()
                    .GetCustomAttributes<PrePostHandlerAttribute>(true)
                    .ToList();

                // Run all pre-handler attributes.
                foreach (PrePostHandlerAttribute attribute in prepostHandlers)
                    attribute.BeforeHandler(parseResult.Command);

                // Execute the command handler.
                return await parseResult.Command.HandleCommandAsync(parseResult).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Run the exception through all attribute error handlers.
                int? attributeErrorCode = null;
                if (prepostHandlers is not null)
                {
                    foreach (PrePostHandlerAttribute attribute in prepostHandlers)
                        attributeErrorCode = attribute.OnException(ex, parseResult?.Command);
                }

                // Run the configured error handler.
                int errorCode = (ErrorHandler ?? new DefaultErrorHandler()).HandleError(ex);
                DebugOutput.Write(ex);

                // Display help if so configured.
                if (_displayHelpOnError)
                    HelpBuilder.DisplayHelp(parseResult?.Command ?? this);

                // If the attributes have returned an error code, use that, otherwise use the error
                // code returned from the error handler.
                return attributeErrorCode ?? errorCode;
            }
            finally
            {
                // Run all post-handler attributes.
                if (prepostHandlers is not null)
                {
                    foreach (PrePostHandlerAttribute attribute in prepostHandlers)
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
        public Task<int> RunAsync(params string[] args)
        {
            return RunAsync((IEnumerable<string>)args);
        }

        /// <summary>
        ///     Runs the console program after parsing the command line args.
        /// </summary>
        /// <returns>
        ///     The numeric code that represents the result of the console program execution.
        /// </returns>
        public Task<int> RunWithCommandLineArgsAsync()
        {
            return RunAsync(Environment.GetCommandLineArgs().Skip(1));
        }

        /// <summary>
        ///     Repeatedly prompts the user for the program args and runs the console program until the
        ///     user enters nothing.
        ///     <para/>
        ///     This method is useful when debugging the application.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user to enter the args.</param>
        /// <param name="condition">
        ///     A delegate that returns whether to run the debug behavior or the default behaviour.
        ///     <para/>
        ///     If not specified, an environment variable named <c>PromptArgs</c> is checked for a value
        ///     of <c>true</c>, and if it the case, the debug behavior is run.
        /// </param>
        /// <param name="defaultBehavior">
        ///     Specifies the default behavior to run if the <paramref name="condition"/> delegate
        ///     return <c>false</c>.
        ///     <para/>
        ///     If not specified, the <see cref="RunWithCommandLineArgsAsync"/> method is called to run
        ///     the program with the command line args.
        /// </param>
        /// <returns>
        ///     The numeric code that represents the result of the console program execution.
        /// </returns>
        public async Task<int> RunDebugAsync(string prompt = "Enter args:",
            Func<bool> condition = null,
            Func<ConsoleProgram, Task<int>> defaultBehavior = null)
        {
            // Assign the default condition, if one is not specified.
            condition ??= () =>
            {
                string promptArgs = Environment.GetEnvironmentVariable("PromptArgs");
                return string.Equals(promptArgs, true.ToString(), StringComparison.OrdinalIgnoreCase);
            };

            // Assign the default behavior, if one is not specified.
            defaultBehavior ??= async program => await program.RunWithCommandLineArgsAsync().ConfigureAwait(false);

            if (!condition())
                return await defaultBehavior(this).ConfigureAwait(false);

            // Assign a default prompt string, if one if not specified.
            prompt ??= "Enter args:";

            Console.Write($"{prompt} {Name} ");
            string args = Console.ReadLine();
            while (!string.IsNullOrEmpty(args))
            {
                IEnumerable<string> tokens = Parser.Parser.Tokenize(args);
                await RunAsync(tokens).ConfigureAwait(false);

                Console.WriteLine();
                Console.Write($"{prompt} {Name} ");
                args = Console.ReadLine();
            }

            return 0;
        }

        /// <summary>
        ///     Add help options to the command as universal options. These options will be added to
        ///     all commands.
        /// </summary>
        /// <returns>The <see cref="Option"/> that represents help.</returns>
        protected sealed override IEnumerable<Option> GetUniversalOptions()
        {
            HelpBuilder helpBuilder = HelpBuilder;

            yield return new Option(helpBuilder.AllNames.ToArray())
                .UsedAsFlag()
                .UnderGroups(int.MinValue)
                .HideHelp();
        }

        private static ParserStyle.ArgStyle CreateArgStyle(ArgStyle argStyle)
        {
            return argStyle switch
            {
                ArgStyle.Unix => new ParserStyle.UnixArgStyle(),
                ArgStyle.Windows => new ParserStyle.WindowsArgStyle(),
                _ => throw new NotSupportedException($"Unsupported argument style: '{argStyle}'."),
            };
        }

        private static void AssignOptionProperties(IParseResult parseResult, Options args)
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
                    AssignProperty(parseResult.Command, arg.AssignedProperty, value);
            }
        }

        private static void AssignArgumentProperties(IParseResult parseResult, Arguments arguments)
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
                    AssignProperty(parseResult.Command, argument.AssignedProperty, value);
            }
        }

        private static void AssignProperty(Command command, PropertyInfo property, object value)
        {
            if (!property.IsCollectionProperty())
            {
                property.SetValue(command, value);
                return;
            }

            object listObj = property.GetValue(command);
            if (listObj is null)
            {
                if (!property.CanWrite)
                {
                    string errorMessage = $"The collection property {property.Name} on command {command.GetType()} "
                        + "does not have a setter, so the framework cannot initialize it. You will need to initialize "
                        + "the property yourself.";
                    throw new ParserException(-1, errorMessage);
                }

                property.SetValue(command, value);
            }
            else
            {
                MethodInfo addMethod = listObj.GetType().GetMethods()
                    .First(mi => mi.Name.Equals("Add") && mi.GetParameters().Length == 1);
                foreach (object item in (IEnumerable)value)
                    addMethod.Invoke(listObj, new[] { item });
            }
        }
    }
}
