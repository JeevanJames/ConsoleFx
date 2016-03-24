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

using System.Collections.Generic;
using System.Linq;

using ConsoleFx.Parser.Config;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;

namespace ConsoleFx.Parser
{
    public class Parser
    {
        public Parser(ParserStyle parserStyle, ArgGrouping grouping = ArgGrouping.DoesNotMatter, object scope = null)
        {
            ParserStyle = parserStyle;
            Grouping = grouping;
            Scope = scope;
        }

        public ParserStyle ParserStyle { get; }

        /// <summary>
        ///     Specifies how the args should be grouped.
        ///     Note: This can be overridden by the parser style at runtime.
        /// </summary>
        public ArgGrouping Grouping { get; set; }

        /// <summary>
        ///     The object instance to write argument and option values when parsing the command-line args.
        /// </summary>
        public object Scope { get; set; }

        public ConfigReader ConfigReader { get; set; }

        /// <summary>
        ///     The implicit <see cref="Command" /> instance that is the root of all commands and args.
        /// </summary>
        private Command RootCommand { get; } = new Command();

        public Arguments Arguments => RootCommand.Arguments;
        public Options Options => RootCommand.Options;
        public Commands Commands => RootCommand.Commands;

        public Argument AddArgument(bool optional = false) => RootCommand.AddArgument(optional);

        public Option AddOption(string name, string shortName = null, bool caseSensitive = false,
            int order = int.MaxValue) =>
                RootCommand.AddOption(name, shortName, caseSensitive, order);

        /// <summary>
        ///     This is the co-ordinating method that accepts a set of string tokens and performs all the
        ///     necessary parsing tasks.
        /// </summary>
        public void Parse(IEnumerable<string> tokens)
        {
            IReadOnlyList<string> tokenList = tokens.ToList();

            Run run = CreateRun(tokenList);
            IReadOnlyList<Option> justOptions = run.Options.Select(o => o.Option).ToList();

            //Even though the caller can define the grouping, the parser style can override it based
            //on the available options and arguments. See the UnixParserStyle class for an example.
            Grouping = ParserStyle.GetGrouping(Grouping, justOptions, run.Arguments);

            //Validate all the available options based on the parser style rules.
            //See the UnixParserStyle for an example.
            ParserStyle.ValidateDefinedOptions(justOptions);

            //Identify all tokens as options or arguments. Identified option details are stored in
            //the Option instance itself. Identified arguments are returned from the method.
            IEnumerable<string> specifiedArguments = ParserStyle.IdentifyTokens(tokens, run.Options, Grouping, Scope);

            if (ConfigReader != null)
                specifiedArguments = ConfigReader.Run(specifiedArguments, Options);

            //Process the specified options and arguments.
            ProcessOptions(run.Options);
            ProcessArguments(specifiedArguments.ToList());
        }

        private Run CreateRun(IReadOnlyList<string> tokens)
        {
            var run = new Run();

            Command currentCommand = RootCommand;

            //Go through the tokens and find all the subcommands and their arguments and options.
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];

                run.Options.AddRange(currentCommand.Options.Select(o => new OptionRun(o)));

                //Check if subcommand exists under the current command with the token as a name.
                Command subcommand = currentCommand.Commands[token];

                if (subcommand != null)
                {
                    //Add the command and move to the next command level.
                    run.Commands.Add(subcommand);
                    currentCommand = subcommand;
                } else
                {
                    //Only add the arguments from the current command to the run if a subcommand is not specified.
                    //Arguments from the innermost command can only be used for a run. If arguments
                    //from different levels of commands are used, then the correct order of the commands is ambiguous.
                    foreach (Argument argument in currentCommand.Arguments)
                        run.Arguments.Add(argument);

                    //All tokens from the current token are considered the args for the command.
                    run.Tokens = new List<string>(tokens.Count - i + 1);
                    for (int j = i; j < tokens.Count; j++)
                        run.Tokens.Add(tokens[j]);

                    //We're done with subcommands. Break out of this look.
                    break;
                }
            }

            return run;
        }

        private void ProcessOptions(IReadOnlyList<OptionRun> options)
        {
            foreach (OptionRun option in options)
            {
                int occurences = option.Occurences;

                if (option.Option.Usage.MinOccurences > 0 && occurences == 0)
                {
                    throw new ParserException(ParserException.Codes.RequiredOptionAbsent,
                        string.Format(Messages.RequiredOptionAbsent, option.Option.Name));
                }
                if (occurences < option.Option.Usage.MinOccurences)
                {
                    throw new ParserException(ParserException.Codes.TooFewOptions,
                        string.Format(Messages.TooFewOptions, option.Option.Name, option.Option.Usage.MinOccurences));
                }
                if (occurences > option.Option.Usage.MaxOccurences)
                {
                    throw new ParserException(ParserException.Codes.TooManyOptions,
                        string.Format(Messages.TooManyOptions, option.Option.Name, option.Option.Usage.MaxOccurences));
                }

                if (occurences > 0 && option.Option.Usage.MinParameters > 0 && option.Parameters.Count == 0)
                {
                    throw new ParserException(ParserException.Codes.RequiredParametersAbsent,
                        string.Format(Messages.RequiredParametersAbsent, option.Option.Name));
                }
                if (occurences > 0 && option.Option.Usage.MinParameters == 0 && option.Option.Usage.MaxParameters == 0 &&
                    option.Parameters.Count > 0)
                {
                    throw new ParserException(ParserException.Codes.InvalidParametersSpecified,
                        string.Format(Messages.InvalidParametersSpecified, option.Option.Name));
                }

                //TODO: Check against MinParameters and MaxParameters

                //If the option has been used and it has parameters and validators, then validate
                //all parameters.
                if (option.Occurences > 0 && option.Parameters.Count > 0 && option.Option.Validators.Count > 0)
                {
                    var parameterIndex = 0;
                    foreach (string parameter in option.Parameters)
                    {
                        IEnumerable<Validator> validatorsByIndex = option.Option.Validators.GetValidators(parameterIndex);
                        if (validatorsByIndex != null)
                        {
                            foreach (Validator validator in validatorsByIndex)
                                validator.Validate(parameter);
                        }

                        validatorsByIndex = option.Option.Validators.GetValidators();
                        if (validatorsByIndex != null)
                        {
                            foreach (Validator validator in validatorsByIndex)
                                validator.Validate(parameter);
                        }

                        parameterIndex++;
                    }
                }

                if (option.Occurences > 0)
                    option.Option.Handler(option.Parameters.ToArray(), Scope);
            }
        }

        /// <summary>
        ///     Process the specified arguments by verifying their usage, validating them and executing
        ///     their handlers.
        /// </summary>
        private void ProcessArguments(IReadOnlyList<string> specifiedArguments)
        {
            if (Arguments.Count == 0)
                return;

            //Throw exception of number of specified arguments is greater than number of defined arguments.
            if (specifiedArguments.Count > Arguments.Count)
            {
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments,
                    Messages.InvalidNumberOfArguments);
            }

            //Find the number of arguments that are required.
            var requiredArgumentCount = 0;
            while (requiredArgumentCount < Arguments.Count && !Arguments[requiredArgumentCount].IsOptional)
                requiredArgumentCount++;

            //Throw exception if not enough required arguments are specified.
            if (specifiedArguments.Count < requiredArgumentCount)
            {
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments,
                    Messages.InvalidNumberOfArguments);
            }

            //Iterate through all specified arguments and validate.
            //If validated, run the argument handler.
            for (var i = 0; i < specifiedArguments.Count; i++)
            {
                string argumentValue = specifiedArguments[i];
                Argument argument = Arguments[i];
                foreach (Validator validator in argument.Validators)
                    validator.Validate(argumentValue);

                argument.Handler(argumentValue, Scope);
            }
        }
    }
}
