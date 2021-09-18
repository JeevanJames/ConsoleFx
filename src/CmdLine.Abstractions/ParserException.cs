// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents errors that occur while parsing command line arguments.
    /// </summary>
    [Serializable]
    public class ParserException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParserException"/> class with an error code
        ///     and message.
        /// </summary>
        /// <param name="errorCode">
        ///     A machine readable code for the specific error that occurred.
        /// </param>
        /// <param name="message">The error message.</param>
        public ParserException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParserException"/> class with an error code
        ///     and message.
        /// </summary>
        /// <param name="errorCode">
        ///     A machine readable code for the specific error that occurred.
        /// </param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception.
        /// </param>
        public ParserException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParserException"/> class with serialized
        ///     data.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo" /> that holds the serialized object data about the
        ///     exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext" /> that contains contextual information about the
        ///     source or destination.
        /// </param>
        protected ParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///     Gets the type of error that occurred so further logic can be applied to handling it.
        ///     <para/>
        ///     A positive error code denotes a functional error.
        ///     <para/>
        ///     A negative error code denotes an unexpected internal error code, which typically means
        ///     a bug in the application or framework.
        /// </summary>
        public int ErrorCode { get; }

        public static class Codes
        {
            public const int InvalidOptionSpecified = PublicErrorCodeBase + 2;
            public const int InvalidOptionParametersSpecified = PublicErrorCodeBase + 3;
            public const int RequiredOptionAbsent = PublicErrorCodeBase + 4;
            public const int OptionsBeforeParameters = PublicErrorCodeBase + 5;
            public const int OptionsAfterParameters = PublicErrorCodeBase + 6;
            public const int TooFewOptions = PublicErrorCodeBase + 7;
            public const int TooManyOptions = PublicErrorCodeBase + 8;
            public const int InvalidOptionParameterSpecifier = PublicErrorCodeBase + 9;
            public const int InvalidNumberOfArguments = PublicErrorCodeBase + 10;
            public const int RequiredParametersAbsent = PublicErrorCodeBase + 11;
            public const int InvalidParametersSpecified = PublicErrorCodeBase + 12;
            public const int ValidationFailure = PublicErrorCodeBase + 13;

            public const int RequiredArgumentsDefinedAfterOptional = InternalErrorCodeBase - 1;

            private const int PublicErrorCodeBase = 0;
            private const int InternalErrorCodeBase = 0;
        }
    }
}
