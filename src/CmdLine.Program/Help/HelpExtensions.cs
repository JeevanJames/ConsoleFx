// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace ConsoleFx.CmdLine.Help
{
    /// <summary>
    ///     Provides extension methods to add standard help metadata to various <see cref="Arg"/>
    ///     objects.
    ///     <para/>
    ///     These are just the standard help metadata. Custom help builders are free to add other
    ///     metadata or use different metadata.
    /// </summary>
    public static class HelpExtensions
    {
        /// <summary>
        ///     Sets a <paramref name="description"/> and optional <paramref name="name"/> for the
        ///     specified <paramref name="argument"/>.
        /// </summary>
        /// <param name="argument">The <see cref="Argument"/> to which to add the help metadata.</param>
        /// <param name="description">The description of the argument.</param>
        /// <param name="name">The name of the argument.</param>
        /// <returns>The instance of the <see cref="Argument"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the description is <c>null</c>.
        /// </exception>
        public static Argument Help(this Argument argument, string description, string name = null)
        {
            if (argument is null)
                throw new ArgumentNullException(nameof(argument));
            if (description is null)
                throw new ArgumentNullException(nameof(description));

            var metadata = (IMetadataObject)argument;
            metadata[HelpMetadataKey.Description] = description;
            if (!string.IsNullOrWhiteSpace(name))
                metadata[HelpMetadataKey.Name] = name;
            return argument;
        }

        /// <summary>
        ///     Sets a <paramref name="description"/> and optional <paramref name="name"/> for the
        ///     specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to which to add the help metadata.</param>
        /// <param name="description">The description of the command.</param>
        /// <param name="name">The name of the command.</param>
        /// <returns>The instance of the <see cref="Command"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="description"/> is <c>null</c>.
        /// </exception>
        public static Command Help(this Command command, string description, string name = null)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));
            if (description is null)
                throw new ArgumentNullException(nameof(description));

            var metadata = (IMetadataObject)command;
            metadata[HelpMetadataKey.Description] = description;
            if (!string.IsNullOrWhiteSpace(name))
                metadata[HelpMetadataKey.Name] = name;
            return command;
        }

        /// <summary>
        ///     Sets a <paramref name="description"/> for the specified <paramref name="option"/>.
        /// </summary>
        /// <param name="option">The <see cref="Option"/> to which to add the help metadata.</param>
        /// <param name="description">The description of the option.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="description"/> is <c>null</c>.
        /// </exception>
        public static Option Help(this Option option, string description)
        {
            if (option is null)
                throw new ArgumentNullException(nameof(option));
            if (description is null)
                throw new ArgumentNullException(nameof(description));

            var metadata = (IMetadataObject)option;
            metadata[HelpMetadataKey.Description] = description;
            return option;
        }

        /// <summary>
        ///     Sets a category <paramref name="name"/> and <paramref name="description"/> for the
        ///     specified <paramref name="arg"/>.
        ///     <para/>
        ///     Help builders can use category metadata to group args when displaying help.
        /// </summary>
        /// <typeparam name="TArg">The type of the <see cref="Arg"/>.</typeparam>
        /// <param name="arg">
        ///     The <see cref="Command"/>, <see cref="Option"/> or <see cref="Argument"/> to which to
        ///     add the help category metadata.
        /// </param>
        /// <param name="name">The category name to assign.</param>
        /// <param name="description">The optional category description to assign.</param>
        /// <returns>The instance of the <see cref="Arg"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public static TArg Category<TArg>(this TArg arg, string name, string description = null)
            where TArg : Arg, IMetadataObject
        {
            if (arg is null)
                throw new ArgumentNullException(nameof(arg));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            arg[HelpMetadataKey.CategoryName] = name;
            arg[HelpMetadataKey.CategoryDescription] = description;
            return arg;
        }

        /// <summary>
        ///     Sets a help metadata on the specified <paramref name="arg"/> to control the order in
        ///     which the arg will be displayed in the help. If the arg belongs to a category, this
        ///     metadata will control the order in which it is displayed in relation to other args of
        ///     the same category.
        /// </summary>
        /// <typeparam name="TArg">The type of the <see cref="Arg"/>.</typeparam>
        /// <param name="arg">
        ///     The <see cref="Command"/>, <see cref="Option"/> or <see cref="Argument"/> to which to
        ///     add the help metadata.
        /// </param>
        /// <param name="order">The order of the arg.</param>
        /// <returns>The instance of the <see cref="Arg"/>.</returns>
        public static TArg Order<TArg>(this TArg arg, int order)
            where TArg : Arg, IMetadataObject
        {
            if (arg is null)
                throw new ArgumentNullException(nameof(arg));

            arg[HelpMetadataKey.Order] = order.ToString(CultureInfo.InvariantCulture);
            return arg;
        }

        /// <summary>
        ///     Sets a help metadata on the specified <paramref name="arg"/> to indicate that help for
        ///     the arg should not be displayed.
        /// </summary>
        /// <typeparam name="TArg">The type of the <see cref="Arg"/>.</typeparam>
        /// <param name="arg">
        ///     The <see cref="Command"/>, <see cref="Option"/> or <see cref="Argument"/> to which to
        ///     add the help metadata.
        /// </param>
        /// <returns>The instance of the <see cref="Arg"/>.</returns>
        public static TArg HideHelp<TArg>(this TArg arg)
            where TArg : Arg, IMetadataObject
        {
            if (arg is null)
                throw new ArgumentNullException(nameof(arg));

            arg[HelpMetadataKey.Hide] = true;
            return arg;
        }
    }
}
