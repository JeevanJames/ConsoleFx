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

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parsers.Validators;
using System.Collections.Generic;

namespace ConsoleFx.Parsers
{
    public class Parser<TStyle>
        where TStyle : ParserStyle, new()
    {
        public Arguments Arguments { get; } = new Arguments();
        public Options Options { get; } = new Options();
        public Behaviors Behaviors { get; } = new Behaviors();

        public Argument AddArgument(bool optional = false)
        {
            var argument = new Argument
            {
                IsOptional = optional
            };
            Arguments.Add(argument);
            return argument;
        }

        public Option AddOption(string name, string shortName = null, OptionRequirement? requirement = null, int minOccurences = 0,
            int maxOccurences = 1, int? expectedOccurences = null, int minParameters = 0, int maxParameters = 0,
            int? expectedParameters = null, bool caseSensitive = false, int order = int.MaxValue)
        {
            var option = new Option(name)
            {
                CaseSensitive = caseSensitive,
                Order = order,
            };
            if (!string.IsNullOrWhiteSpace(shortName))
                option.ShortName = shortName;

            if (requirement.HasValue)
                option.Usage.Requirement = requirement.Value;
            else if (expectedOccurences.HasValue)
                option.Usage.ExpectedOccurences = expectedOccurences.Value;
            else
            {
                option.Usage.MinOccurences = minOccurences;
                option.Usage.MaxOccurences = maxOccurences;
            }

            if (expectedParameters.HasValue)
                option.Usage.ExpectedParameters = expectedParameters.Value;
            else
            {
                option.Usage.MinParameters = minParameters;
                option.Usage.MaxParameters = maxParameters;
            }

            Options.Add(option);
            return option;
        }

        /// <summary>
        /// This is the co-ordinating method that accepts a set of string tokens and performs all the
        /// necessary parsing tasks.
        /// This method is protected because the derived specialized classes might want to use different
        /// terminology for parsing. For example, the ConsoleProgram class uses a method called Run
        /// to start execution. The Run method calls Parse internally to do the main work. Similarly,
        /// other derived classes like DeclarativeConsoleProgram have their own conventions.
        /// </summary>
        public void Parse(IEnumerable<string> tokens)
        {
            ClearPreviousRun();

            //Identify each token passed and add to the SpecifiedValues property.
            var parserStyle = new TStyle();
            var specifiedArguments = new List<string>(parserStyle.IdentifyTokens(tokens, Options, Behaviors));

            //Process the specified options and arguments.
            ProcessOptions();
            ProcessArguments(specifiedArguments);
        }

        /// <summary>
        /// Clears any parsing data from a 
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
                if (option.Usage.MinOccurences > 0 && option.Run.Occurences == 0)
                    throw new ParserException(ParserException.Codes.RequiredOptionAbsent, Parser.Messages.RequiredOptionAbsent, option.Name);
                if (option.Run.Occurences < option.Usage.MinOccurences)
                    throw new ParserException(ParserException.Codes.TooFewOptions, Parser.Messages.TooFewOptions, option.Name, option.Usage.MinOccurences);
                if (option.Run.Occurences > option.Usage.MaxOccurences)
                    throw new ParserException(ParserException.Codes.TooManyOptions, Parser.Messages.TooManyOptions, option.Name, option.Usage.MaxOccurences);

                if (option.Usage.MinParameters > 0 && option.Run.Parameters.Count == 0)
                    throw new ParserException(ParserException.Codes.RequiredParametersAbsent, Parser.Messages.RequiredParametersAbsent, option.Name);
                if (option.Usage.MinParameters == 0 && option.Usage.MaxParameters == 0 && option.Run.Parameters.Count > 0)
                    throw new ParserException(ParserException.Codes.InvalidParametersSpecified, Parser.Messages.InvalidParametersSpecified, option.Name);

                //TODO: Check against MinParameters and MaxParameters

                //If the option has been used and it has parameters and validators, then validate
                //all parameters.
                if (option.Run.Occurences > 0 && option.Run.Parameters.Count > 0 && option.Validators.Count > 0)
                {
                    int parameterIndex = 0;
                    foreach (string parameter in option.Run.Parameters)
                    {
                        OptionParameterValidators validatorsByIndex = option.Validators[parameterIndex];
                        if (validatorsByIndex != null)
                        {
                            foreach (BaseValidator validator in validatorsByIndex.Validators)
                                validator.Validate(parameter);
                        }

                        validatorsByIndex = option.Validators[-1];
                        if (validatorsByIndex != null)
                        {
                            foreach (BaseValidator validator in validatorsByIndex.Validators)
                                validator.Validate(parameter);
                        }

                        parameterIndex++;
                    }
                }

                if (option.Run.Occurences > 0)
                    option.Handler(option.Run.Parameters.ToArray());
            }
        }

        /// <summary>
        /// Process the specified arguments by verifying their usage, validating them and executing
        /// their handlers.
        /// </summary>
        private void ProcessArguments(List<string> specifiedArguments)
        {
            if (Arguments.Count == 0)
                return;

            //Throw exception of number of specified arguments is greater than number of defined arguments.
            if (specifiedArguments.Count > Arguments.Count)
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments, Parser.Messages.InvalidNumberOfArguments);

            //Find the number of arguments that are required.
            int requiredArgumentCount = 0;
            while (requiredArgumentCount < Arguments.Count && !Arguments[requiredArgumentCount].IsOptional)
                requiredArgumentCount++;

            //Throw exception if not enough required arguments are specified.
            if (specifiedArguments.Count < requiredArgumentCount)
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments, Parser.Messages.InvalidNumberOfArguments);

            //Iterate through all specified arguments and validate.
            //If validated, run the argument handler.
            for (int i = 0; i < specifiedArguments.Count; i++)
            {
                string argumentValue = specifiedArguments[i];
                Argument argument = Arguments[i];
                foreach (BaseValidator validator in argument.Validators)
                    validator.Validate(argumentValue);

                argument.Handler(argumentValue);
            }
        }
    }
}