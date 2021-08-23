// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine.Program
{
    public abstract class ErrorHandler
    {
        /// <summary>
        ///     Handles the specified error.
        /// </summary>
        /// <param name="ex">The error to be handled.</param>
        /// <returns>The status code corresponding to the error.</returns>
        public abstract int HandleError(Exception ex);
    }
}
