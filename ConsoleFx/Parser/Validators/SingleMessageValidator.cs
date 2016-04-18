namespace ConsoleFx.Parser.Validators
{
    /// <summary>
    ///     Base class for validators that only have one possible type of validation failure. In this case, the class provides
    ///     an OOTB Message property.
    /// </summary>
    /// <typeparam name="T">The actual type of the value being validated.</typeparam>
    public abstract class SingleMessageValidator<T> : Validator<T>
    {
        protected SingleMessageValidator(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The error message to be displayed if the validation fails.
        /// </summary>
        public string Message { get; set; }
    }
}