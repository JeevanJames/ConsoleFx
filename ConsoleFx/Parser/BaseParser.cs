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

using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using System.Collections.Generic;

namespace ConsoleFx.Parser
{
    public abstract class BaseParser<TStyle>
        where TStyle : ParserStyle, new()
    {
        protected Arguments Arguments { get; } = new Arguments();
        protected Options Options { get; } = new Options();
        protected Behaviors Behaviors { get; } = new Behaviors();

        /// <summary>
        /// This is the co-ordinating method that accepts a set of string tokens and performs all the
        /// necessary parsing tasks.
        /// This method is protected because the derived specialized classes might want to use different
        /// terminology for parsing. For example, the ConsoleProgram class uses a method called Run
        /// to start execution. The Run method calls Parse internally to do the main work. Similarly,
        /// other derived classes like DeclarativeConsoleProgram have their own conventions.
        /// </summary>
        protected void Parse(IEnumerable<string> tokens)
        {
            ClearPreviousRun();

            //Identify each token passed and add to the SpecifiedValues property.
            var parserStyle = new TStyle();
            Behaviors.Grouping = parserStyle.GetGrouping(Behaviors.Grouping, Options, Arguments);
            parserStyle.ValidateDefinedOptions(Options);
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
                option.Scope = Behaviors.Scope;

                int occurences = option.Run.Occurences;

                if (option.Usage.MinOccurences > 0 && occurences == 0)
                    throw new ParserException(ParserException.Codes.RequiredOptionAbsent, string.Format(Messages.RequiredOptionAbsent, option.Name));
                if (occurences < option.Usage.MinOccurences)
                    throw new ParserException(ParserException.Codes.TooFewOptions, string.Format(Messages.TooFewOptions, option.Name, option.Usage.MinOccurences));
                if (occurences > option.Usage.MaxOccurences)
                    throw new ParserException(ParserException.Codes.TooManyOptions, string.Format(Messages.TooManyOptions, option.Name, option.Usage.MaxOccurences));

                if (occurences > 0 && option.Usage.MinParameters > 0 && option.Run.Parameters.Count == 0)
                    throw new ParserException(ParserException.Codes.RequiredParametersAbsent, string.Format(Messages.RequiredParametersAbsent, option.Name));
                if (occurences > 0 && option.Usage.MinParameters == 0 && option.Usage.MaxParameters == 0 && option.Run.Parameters.Count > 0)
                    throw new ParserException(ParserException.Codes.InvalidParametersSpecified, string.Format(Messages.InvalidParametersSpecified, option.Name));

                //TODO: Check against MinParameters and MaxParameters

                //If the option has been used and it has parameters and validators, then validate
                //all parameters.
                if (option.Run.Occurences > 0 && option.Run.Parameters.Count > 0 && option.Validators.Count > 0)
                {
                    int parameterIndex = 0;
                    foreach (string parameter in option.Run.Parameters)
                    {
                        IEnumerable<BaseValidator> validatorsByIndex = option.Validators.GetValidators(parameterIndex);
                        if (validatorsByIndex != null)
                        {
                            foreach (BaseValidator validator in validatorsByIndex)
                                validator.Validate(parameter);
                        }

                        validatorsByIndex = option.Validators.GetValidators(-1);
                        if (validatorsByIndex != null)
                        {
                            foreach (BaseValidator validator in validatorsByIndex)
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
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments, Messages.InvalidNumberOfArguments);

            //Find the number of arguments that are required.
            int requiredArgumentCount = 0;
            while (requiredArgumentCount < Arguments.Count && !Arguments[requiredArgumentCount].IsOptional)
                requiredArgumentCount++;

            //Throw exception if not enough required arguments are specified.
            if (specifiedArguments.Count < requiredArgumentCount)
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments, Messages.InvalidNumberOfArguments);

            //Iterate through all specified arguments and validate.
            //If validated, run the argument handler.
            for (int i = 0; i < specifiedArguments.Count; i++)
            {
                string argumentValue = specifiedArguments[i];
                Argument argument = Arguments[i];
                argument.Scope = Behaviors.Scope;
                foreach (BaseValidator validator in argument.Validators)
                    validator.Validate(argumentValue);

                argument.Handler(argumentValue);
            }
        }
    }
}