// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Returns the specified error code from the command if any of the specified exceptions or
    ///     their derivatives were thrown.
    /// </summary>
    public sealed class ErrorCodeAttribute : PrePostHandlerAttribute
    {
        public ErrorCodeAttribute(int errorCode, params Type[] exceptionTypes)
        {
            if (errorCode == 0)
                throw new ArgumentException("An error code of 0 represents a success code. It cannot be an error code. Use a non-zero value.", nameof(errorCode));
            if (exceptionTypes is null)
                throw new ArgumentNullException(nameof(exceptionTypes));
            if (exceptionTypes.Length == 0)
                throw new ArgumentException("Specify at least one exception type.", nameof(exceptionTypes));
            if (exceptionTypes.Any(type => type is null || !typeof(Exception).IsAssignableFrom(type)))
                throw new ArgumentException("There are either null types or types that are not exceptions.", nameof(exceptionTypes));

            ErrorCode = errorCode;
            ExceptionTypes = exceptionTypes;
        }

        /// <summary>
        ///     Gets the error code to return if any of the specified exceptions was thrown.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        ///     Gets the list of exception types that will return the specified error code if any of
        ///     the exceptions are thrown.
        /// </summary>
        public IReadOnlyList<Type> ExceptionTypes { get; }

        public override int? OnException(Exception ex, Command command)
        {
            return ExceptionTypes.Any(type => type.IsAssignableFrom(ex.GetType())) ? ErrorCode : null;
        }
    }
}
