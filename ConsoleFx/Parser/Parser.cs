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

using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;

namespace ConsoleFx.Parser
{
    public class Parser
    {
        public Parser(ParserStyle parserStyle, CommandGrouping grouping = CommandGrouping.DoesNotMatter, object scope = null)
        {
            ParserStyle = parserStyle;
            Grouping = grouping;
            Scope = scope;
        }

        public ParserStyle ParserStyle { get; }

        /// <summary>
        ///     Specifies how the args should be grouped.
        ///     Note: This can be overridden by the parser style.
        /// </summary>
        public CommandGrouping Grouping { get; set; }

        /// <summary>
        ///     The object instance to write argument and option values when parsing the command-line args.
        ///     If null, then these values are written to static properties.
        /// </summary>
        public object Scope { get; set; }

        public Arguments Arguments { get; } = new Arguments();
        public Options Options { get; } = new Options();

        public Argument AddArgument(bool optional = false)
        {
            var argument = new Argument {
                IsOptional = optional
            };
            Arguments.Add(argument);
            return argument;
        }

        public Option AddOption(string name, string shortName = null, bool caseSensitive = false,
            int order = int.MaxValue)
        {
            var option = new Option(name) {
                CaseSensitive = caseSensitive,
                Order = order
            };
            if (!string.IsNullOrWhiteSpace(shortName))
                option.ShortName = shortName;

            Options.Add(option);
            return option;
        }

        /// <summary>
        ///     This is the co-ordinating method that accepts a set of string tokens and performs all the
        ///     necessary parsing tasks.
        /// </summary>
        public void Parse(IEnumerable<string> tokens)
        {
            ClearPreviousRun();

            //Even though the caller can define the grouping, the parser style can override it based
            //on the available options and arguments.
            Grouping = ParserStyle.GetGrouping(Grouping, Options, Arguments);

            //Validate all the available options based on the parser style rules.
            ParserStyle.ValidateDefinedOptions(Options);
            var specifiedArguments = new List<string>(ParserStyle.IdentifyTokens(tokens, Options, new Behaviors { Grouping = Grouping, Scope = Scope }));

            //Process the specified options and arguments.
            ProcessOptions();
            ProcessArguments(specifiedArguments);
        }

        /// <summary>
        ///     Clears any parsing data from a previous parse run.
        /// </summary>
        private void ClearPreviousRun()
        {
            foreach (Option option in Options)
                option.ClearRun();
        }

        private void ProcessOptions()
        {
            foreach (Option option in Options)
            {
                int occurences = option.Run.Occurences;

                if (option.Usage.MinOccurences > 0 && occurences == 0)
                {
                    throw new ParserException(ParserException.Codes.RequiredOptionAbsent,
                        string.Format(Messages.RequiredOptionAbsent, option.Name));
                }
                if (occurences < option.Usage.MinOccurences)
                {
                    throw new ParserException(ParserException.Codes.TooFewOptions,
                        string.Format(Messages.TooFewOptions, option.Name, option.Usage.MinOccurences));
                }
                if (occurences > option.Usage.MaxOccurences)
                {
                    throw new ParserException(ParserException.Codes.TooManyOptions,
                        string.Format(Messages.TooManyOptions, option.Name, option.Usage.MaxOccurences));
                }

                if (occurences > 0 && option.Usage.MinParameters > 0 && option.Run.Parameters.Count == 0)
                {
                    throw new ParserException(ParserException.Codes.RequiredParametersAbsent,
                        string.Format(Messages.RequiredParametersAbsent, option.Name));
                }
                if (occurences > 0 && option.Usage.MinParameters == 0 && option.Usage.MaxParameters == 0 &&
                    option.Run.Parameters.Count > 0)
                {
                    throw new ParserException(ParserException.Codes.InvalidParametersSpecified,
                        string.Format(Messages.InvalidParametersSpecified, option.Name));
                }

                //TODO: Check against MinParameters and MaxParameters

                //If the option has been used and it has parameters and validators, then validate
                //all parameters.
                if (option.Run.Occurences > 0 && option.Run.Parameters.Count > 0 && option.Validators.Count > 0)
                {
                    var parameterIndex = 0;
                    foreach (string parameter in option.Run.Parameters)
                    {
                        IEnumerable<Validator> validatorsByIndex = option.Validators.GetValidators(parameterIndex);
                        if (validatorsByIndex != null)
                        {
                            foreach (Validator validator in validatorsByIndex)
                                validator.Validate(parameter);
                        }

                        validatorsByIndex = option.Validators.GetValidators();
                        if (validatorsByIndex != null)
                        {
                            foreach (Validator validator in validatorsByIndex)
                                validator.Validate(parameter);
                        }

                        parameterIndex++;
                    }
                }

                if (option.Run.Occurences > 0)
                    option.Handler(option.Run.Parameters.ToArray(), Scope);
            }
        }

        /// <summary>
        ///     Process the specified arguments by verifying their usage, validating them and executing
        ///     their handlers.
        /// </summary>
        private void ProcessArguments(List<string> specifiedArguments)
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