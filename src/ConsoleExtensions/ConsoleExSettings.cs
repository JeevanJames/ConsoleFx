// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ConsoleFx.ConsoleExtensions
{
    public sealed class ConsoleExSettings
    {
        private IList<char> _indentationLineBreakChars;

        public ColorResetOption ColorReset { get; set; } = ColorResetOption.ResetAfterCommand;

        /// <summary>
        ///     Gets or sets the character to be used when entering a secret value using the ReadSecret
        ///     methods. The default is <c>'*'</c>.
        ///     <para />
        ///     Changing this value applies globally.
        /// </summary>
        public char SecretMask { get; set; } = '*';

        public IList<char> IndentationLineBreakChars => _indentationLineBreakChars ??= new List<char> { ' ' };
    }

    /// <summary>
    ///     Determines when to automatically reset the console colors when printing a
    ///     <see cref="ColorString"/>.
    /// </summary>
    public enum ColorResetOption
    {
        /// <summary>
        ///     The default - resets the colors after each print command is executed.
        /// </summary>
        ResetAfterCommand,

        /// <summary>
        ///     Don't reset the colors at all. Subsequent print commands will use the same colors that
        ///     were set in the previous print command.
        /// </summary>
        DontReset,

        /// <summary>
        ///     Reset the color after every block of color used in a print command.
        /// </summary>
        ResetAfterColor,
    }
}
