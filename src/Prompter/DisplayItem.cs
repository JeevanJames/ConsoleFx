// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Base class for any item that can be displayed by the prompter.
    ///     <para/>
    ///     This includes questions (<see cref="Question"/>) and static text (<see cref="StaticText"/>).
    /// </summary>
    public abstract class DisplayItem : PromptItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisplayItem"/> class.
        /// </summary>
        /// <param name="message">The <paramref name="message"/> to display to the user.</param>
        protected DisplayItem(Factory<string> message)
        {
            if (!message.IsAssigned)
                throw new ArgumentNullException(nameof(message));
            Message = message;
        }

        /// <summary>
        ///     Gets the message to display to the user for this display item.
        /// </summary>
        internal Factory<string> Message { get; }
    }
}
