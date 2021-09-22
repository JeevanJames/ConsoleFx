// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.Prompter
{
    [DebuggerDisplay("Question: {Name,nq}: {Message.Resolve(),nq}")]
    public abstract class Question : DisplayItem
    {
        private IList<FunctionOrColorString> _instructions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        /// <param name="name">The name of the variable to store the answer.</param>
        /// <param name="message">The message to display to the user.</param>
        protected Question(string name, FunctionOrColorString message)
            : base(message)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify a name for the question.", nameof(name));
            Name = name;
        }

        /// <summary>
        ///     Gets the delegate to call to display the prompt.
        ///     <para/>
        ///     Derived implementations must override this property to provide the behavior of displaying
        ///     the prompt item, and if needed, the behaviors to get an answer.
        /// </summary>
        internal abstract object Ask(dynamic answers);

        /// <summary>
        ///     Gets the name of the variable to store the answer.
        /// </summary>
        public string Name { get; }

        public IList<FunctionOrColorString> Instructions
        {
            get => _instructions ??= new List<FunctionOrColorString>();
            set => _instructions = value;
        }

        internal FunctionOrValue<object> DefaultValue { get; set; }

        internal Validator<object> RawValueValidator { get; set; }

        internal Func<object, object> ConverterFn { get; set; }

        internal Validator<object> ConvertedValueValidator { get; set; }

        internal object Convert(object value) =>
            ConverterFn is not null ? ConverterFn(value) : value;
    }

    public abstract class Question<TRaw, TConverted> : Question
    {
        protected Question(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }

        public Question<TRaw, TConverted> WithInstructions(params FunctionOrColorString[] instructions)
        {
            if (instructions is null)
                throw new ArgumentNullException(nameof(instructions));

            foreach (FunctionOrColorString instruction in instructions)
            {
                if (instruction.IsAssigned)
                    Instructions.Add(instruction);
            }

            return this;
        }

        public new Question<TRaw, TConverted> When(AnswersFunc<bool> canAskFn)
        {
            CanAskFn = canAskFn;
            return this;
        }

        public Question<TRaw, TConverted> DefaultsTo(FunctionOrValue<TConverted> defaultValue)
        {
            DefaultValue = defaultValue;
            return this;
        }

        public Question<TRaw, TConverted> ValidateWith(BasicValidator<TConverted> validator)
        {
            if (validator is null)
                throw new ArgumentNullException(nameof(validator));
            ConvertedValueValidator = (value, _) => validator((TConverted)value);
            return this;
        }

        public Question<TRaw, TConverted> ValidateWith(Validator<TConverted> validator)
        {
            if (validator is null)
                throw new ArgumentNullException(nameof(validator));
            ConvertedValueValidator = (value, ans) => validator((TConverted)value, ans);
            return this;
        }

        public Question<TRaw, TConverted> ValidateInputWith(BasicValidator<TRaw> validator)
        {
            if (validator is null)
                throw new ArgumentNullException(nameof(validator));
            RawValueValidator = (value, _) => validator((TRaw)value);
            return this;
        }

        public Question<TRaw, TConverted> ValidateInputWith(Validator<TRaw> validator)
        {
            if (validator is null)
                throw new ArgumentNullException(nameof(validator));
            RawValueValidator = (value, ans) => validator((TRaw)value, ans);
            return this;
        }

        public Question<TRaw, TConverted> Transform(Func<TRaw, TConverted> converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));
            ConverterFn = raw => converter((TRaw)raw);
            return this;
        }
    }
}
