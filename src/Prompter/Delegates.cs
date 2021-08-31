// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Delegate that returns a value based on the <paramref name="answers"/> already provided
    ///     to earlier questions.
    /// </summary>
    /// <typeparam name="TResult">The type of the returned value.</typeparam>
    /// <param name="answers">The answers already provided to earlier questions.</param>
    public delegate TResult AnswersFunc<out TResult>(dynamic answers);

    public delegate ValidationResult Validator<in TValue>(TValue value, dynamic answers);

    public delegate ValidationResult BasicValidator<in TValue>(TValue value);
}
