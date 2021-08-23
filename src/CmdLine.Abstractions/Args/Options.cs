// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents a collection of options.
    ///     <para/>
    ///     Note: This is not a keyed collection because the key can be one of many names.
    /// </summary>
    public sealed class Options : Args<Option>
    {
        /// <summary>
        ///     Gets the <see cref="Option"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>An of the option, if found, otherwise <c>null</c>.</returns>
        public Option this[string name] => this.FirstOrDefault(item => item.HasName(name));

        /// <inheritdoc />
        protected override void CheckDuplicates(Option obj, int index)
        {
            for (var i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;
                if (ObjectsMatch(obj, this[i]))
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Messages.OptionAlreadyExists, obj.Name), nameof(obj));
            }
        }

        /// <summary>
        ///     Checks whether two <see cref="Option"/> instances are the same.
        /// </summary>
        /// <param name="option1">The first <see cref="Option"/> instance.</param>
        /// <param name="option2">The second <see cref="Option"/> instance.</param>
        /// <returns><c>true</c> if the options match, otherwise <c>false</c>.</returns>
        private bool ObjectsMatch(Option option1, Option option2)
        {
            return option1.AllNames.Any(name => option2.HasName(name));
        }
    }
}
