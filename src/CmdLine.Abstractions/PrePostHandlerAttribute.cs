﻿#region --- License & Copyright Notice ---
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for attributes that can run code before or after a <see cref="Command"/> handler
    ///     is executed, or when an exception occurs during the execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PrePostHandlerAttribute : Attribute
    {
        /// <summary>
        ///     Override this method to run code before the <see cref="Command"/> handler is executed.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to be executed.</param>
        public virtual void BeforeHandler(Command command)
        {
        }

        /// <summary>
        ///     Override this method to run code whenever the <see cref="Command"/> handler throws
        ///     an unhandled exception.
        /// </summary>
        /// <param name="ex">The thrown exception.</param>
        /// <param name="command">The <see cref="Command"/> that was executed.</param>
        /// <returns>Integer exit code that should be returned from the application.</returns>
        public virtual int? OnException(Exception ex, Command command)
        {
            return null;
        }

        /// <summary>
        ///     Override this method to run code after the <see cref="Command"/> handler is executed.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> that was executed.</param>
        public virtual void AfterHandler(Command command)
        {
        }
    }

    /// <summary>
    ///     Restores the current directory to the directory that was current before the command was
    ///     executed.
    /// </summary>
    public sealed class PushDirectoryAttribute : PrePostHandlerAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _originalDirectory;

        public override void BeforeHandler(Command command)
        {
            _originalDirectory = Directory.GetCurrentDirectory();
        }

        public override void AfterHandler(Command command)
        {
            Directory.SetCurrentDirectory(_originalDirectory);
        }
    }

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
            return ExceptionTypes.Any(type => type.IsAssignableFrom(ex.GetType())) ? ErrorCode : (int?)null;
        }
    }
}