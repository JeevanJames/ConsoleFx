using System;

namespace ConsoleFx.Parser.Validators
{
    public class ValidationException : ParserException
    {
        public ValidationException(string message, Type validatorType, string parameterValue) : base(Codes.ValidationFailed, message)
        {
            ValidatorType = validatorType;
            ParameterValue = parameterValue;
        }

        public Type ValidatorType { get; }

        public string ParameterValue { get; }
    }
}