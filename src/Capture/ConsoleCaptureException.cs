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

using System;
using System.Runtime.Serialization;

namespace ConsoleFx.Capture
{
    /// <summary>
    ///     Represents errors that occur during <see cref="ConsoleCapture"/> execution.
    /// </summary>
    [Serializable]
    public sealed class ConsoleCaptureException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleCaptureException"/> class with a
        ///     specified <paramref name="errorCode"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="errorCode">The code that describes the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public ConsoleCaptureException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleCaptureException"/> class with a
        ///     specified <paramref name="errorCode"/>, <paramref name="message"/> and a reference to
        ///     the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorCode">The code that describes the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public ConsoleCaptureException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleCaptureException"/> class with
        ///     serialized data.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo"/> that holds the serialized object data about the
        ///     exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext"/> that contains contextual information about the source
        ///     or destination.
        /// </param>
        private ConsoleCaptureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///     Gets the code that describes the error.
        /// </summary>
        public int ErrorCode { get; }

        public static class Codes
        {
            public const int ProcessStartFailed = 101;
            public const int ProcessAborted = 102;
        }
    }
}
