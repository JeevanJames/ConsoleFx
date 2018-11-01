using System;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class TypedQuestion<T> : Question
    {
        private readonly AskerFn _askerFn;

        internal TypedQuestion(Question question) : base(question.Name, question.Message)
        {
            if (question == null)
                throw new System.ArgumentNullException(nameof(question));
            _askerFn = question.AskerFn;
            CanAskFn = question.CanAskFn;
            Validator = question.Validator;
            ConvertedValueValidator = question.ConvertedValueValidator;
        }

        public TypedQuestion<T> Validate(Func<T, bool> validator)
        {
            ConvertedValueValidator = (value, _) => validator((T)value);
            return this;
        }

        public TypedQuestion<T> Validate(Func<T, dynamic, bool> validator)
        {
            ConvertedValueValidator = (value, ans) => validator((T) value, ans);
            return this;
        }

        internal override AskerFn AskerFn => _askerFn;
    }
}