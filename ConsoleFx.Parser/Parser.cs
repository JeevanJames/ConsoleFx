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
using System.Linq;

namespace ConsoleFx.Parsers
{
    public abstract class Parser<TStyle>
        where TStyle : ParserStyle, new()
    {
        public Arguments Arguments { get; } = new Arguments();
        public Options Options { get; } = new Options();
        public Behaviors Behaviors { get; } = new Behaviors();

        private SpecifiedValues Specified { get; set; }

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
            //Identify each token passed and add to the SpecifiedValues property.
            var parserStyle = new TStyle();
            Specified = parserStyle.IdentifyTokens(tokens, Arguments, Options, Behaviors);

            //For each specified option, do the validations (if any) and then call its handler
            ValidateOptions();
            ExecuteOptionHandler();

            CheckArgumentUsage();
            CheckOptionUsage();
            ValidateArguments();
        }

        #region Commandline parsing methods
        //If all the options are valid, validate the option parameters against any parameter validators
        //decorated on the method.
        private void ValidateOptions()
        {
            foreach (KeyValuePair<string, SpecifiedOptionParameters> specifiedOption in Specified.Options)
            {
                //Note: No need to check for null on the option; we know the available option exists, because we checked for invalid options in the IdentifyTokens method
                Option option = Options[specifiedOption.Key];

                if (option.Validators.Count == 0)
                    continue;

                int parameterIdx = 0;
                foreach (string parameter in specifiedOption.Value)
                {
                    OptionParameterValidators validatorsByIndex = option.Validators[parameterIdx];
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

                    parameterIdx++;
                }
            }
        }

        //Iterate through all the options specified in the command line and executes their option
        //delegates, after performing basic validation.
        private void ExecuteOptionHandler()
        {
            foreach (KeyValuePair<string, SpecifiedOptionParametersCollection> specifiedOption in Properties.Specified.Options)
            {
                //Option should exist at this point, since we already checked for non-declared options
                //in the ValidateOptions method
                Option option = Options[specifiedOption.Key];

                foreach (SpecifiedOptionParameters parameters in specifiedOption.Value)
                {
                    //Attempt to execute the delegate for the specified option. The method can perform
                    //some basic validation, and if it fails, it can throw an exception.
                    option.Handler(parameters.ToArray());
                }
            }
        }

        //Get the usage for each option based on the context, and ensure that the usage rules are met
        private void CheckOptionUsage()
        {
            foreach (Option option in Options)
            {
                OptionUsage optionUsage = option.Usages[Properties.Context];

                //Get the option parameter sets for the given option
                SpecifiedOptionParametersCollection specifiedOptionParametersCollection = Specified.Options[option] ?? new SpecifiedOptionParametersCollection();

                if (optionUsage.MinOccurences > 0 && specifiedOptionParametersCollection.Count == 0)
                    throw new ParserException(ParserException.Codes.RequiredOptionAbsent, Messages.RequiredOptionAbsent, option.Name);

                if (optionUsage.Requirement == OptionRequirement.NotAllowed && specifiedOptionParametersCollection.Count > 0)
                {
                    throw new ParserException(ParserException.Codes.InvalidOptionSpecified, Messages.InvalidOptionSpecified,
                        specifiedOptionParametersCollection[0].OptionName);
                }

                if (optionUsage.MaxOccurences > 0)
                {
                    if (specifiedOptionParametersCollection.Count < optionUsage.MinOccurences)
                        throw new ParserException(ParserException.Codes.TooFewOptions, Messages.TooFewOptions, option.Name,
                            optionUsage.MinOccurences);
                    if (specifiedOptionParametersCollection.Count > optionUsage.MaxOccurences)
                        throw new ParserException(ParserException.Codes.TooManyOptions, Messages.TooManyOptions, option.Name,
                            optionUsage.MaxOccurences);
                }

                foreach (SpecifiedOptionParameters parameters in specifiedOptionParametersCollection)
                {
                    if (optionUsage.MinParameters > 0 && parameters.Count == 0)
                        throw new ParserException(ParserException.Codes.RequiredParametersAbsent, Messages.RequiredParametersAbsent,
                            parameters.OptionName);
                    if (optionUsage.MinParameters == 0 && optionUsage.MaxParameters == 0 && parameters.Count > 0)
                        throw new ParserException(ParserException.Codes.InvalidParametersSpecified,
                            Messages.InvalidParametersSpecified, parameters.OptionName);

                    //TODO: Check against MinParameters and MaxParameters
                }
            }
        }

        //Check the number of arguments specified on the command-line, against the min
        //and max specified by the corresponding ArgumentUsage attribute.
        private void CheckArgumentUsage()
        {
            Arguments arguments = Arguments[Properties.Context];
            if (arguments == null)
                return;

            int requiredArgumentCount = 0;
            while (requiredArgumentCount < arguments.Count && !arguments[requiredArgumentCount].IsOptional)
                requiredArgumentCount++;

            int specifiedArgumentsCount = Specified.Arguments.Count;

            if (specifiedArgumentsCount < requiredArgumentCount || specifiedArgumentsCount > arguments.Count)
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments, Messages.InvalidNumberOfArguments);
        }

        //Validate the arguments against any arguments validators that are decorated on the
        //program class.
        //Also, in the same process, call the argument's handler delegate.
        private void ValidateArguments()
        {
            Arguments arguments = Arguments[Properties.Context];
            if (arguments == null)
                return;

            for (int argumentIdx = 0; argumentIdx < Specified.Arguments.Count; argumentIdx++)
            {
                string argumentValue = Specified.Arguments[argumentIdx];
                Argument argument = arguments[argumentIdx];
                foreach (BaseValidator validator in argument.Validators)
                    validator.Validate(argumentValue);

                argument.Handler(argumentValue);
            }
        }
        #endregion
    }
}