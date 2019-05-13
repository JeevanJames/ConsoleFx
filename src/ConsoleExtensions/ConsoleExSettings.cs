#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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

        public IList<char> IndentationLineBreakChars
        {
            get => _indentationLineBreakChars ?? (_indentationLineBreakChars = new List<char> { ' ' });
            set => _indentationLineBreakChars = value;
        }
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
