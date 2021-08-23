// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Base class for any item that can be added to the <see cref="PrompterFlow"/> collection.
    /// </summary>
    public abstract class PromptItem
    {
        /// <summary>
        ///     Sets a delegate that controls whether this item can be processed or asked (if it is
        ///     a <see cref="Question"/>).
        /// </summary>
        /// <param name="canAskFn">The delegate to set.</param>
        /// <returns>The same <see cref="PromptItem"/> instance.</returns>
        public PromptItem When(AnswersFunc<bool> canAskFn)
        {
            CanAskFn = canAskFn;
            return this;
        }

        /// <summary>
        ///     Gets or sets a delegate indicating whether the <see cref="PromptItem"/> can be asked
        ///     or processed.
        /// </summary>
        internal AnswersFunc<bool> CanAskFn { get; set; }

        /// <summary>
        ///     Helper method to check whether this item can be processed or asked based on the <see cref="CanAskFn"/>
        ///     delegate.
        /// </summary>
        /// <param name="answers">The answers already provided to earlier questions.</param>
        /// <returns><c>True</c> if this item can be processed or asked.</returns>
        internal bool CanAsk(dynamic answers)
        {
            return CanAskFn is null || (bool)CanAskFn(answers);
        }
    }
}
