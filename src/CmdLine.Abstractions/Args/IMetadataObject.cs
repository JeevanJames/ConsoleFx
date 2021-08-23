// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents an object that can contain arbitrary key/value data (metadata).
    /// </summary>
    public interface IMetadataObject
    {
        /// <summary>
        ///     Gets or sets a metadata value.
        /// </summary>
        /// <param name="name">The name of the metadata value.</param>
        /// <returns>The value of the metadata.</returns>
        object this[string name] { get; set; }

        /// <summary>
        ///     Gets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <returns>The metadata value or the default of T if the value does not exist.</returns>
        T Get<T>(string name);

        /// <summary>
        ///     Sets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <param name="value">The value of the metadata to set.</param>
        void Set<T>(string name, T value);
    }
}
