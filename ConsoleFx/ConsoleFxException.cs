#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

namespace ConsoleFx
{
    /// <summary>
    ///     Base class for all exception types in the ConsoleFx library. This class provides an ErrorCode to indicate the
    ///     specific type of exception that occurred.
    /// </summary>
    public abstract class ConsoleFxException : Exception
    {
        /// <summary>
        ///     Identifies the type of error that occurred so further logic can be applied to handling it.
        ///     A positive error code denotes a functional error.
        ///     A negative error code denotes an internal error code, which typically means a bug in the application or framework.
        /// </summary>
        public int ErrorCode { get; }

        protected ConsoleFxException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        protected ConsoleFxException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
