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
using System.Runtime.Serialization;

namespace ConsoleFx.Parsers
{
    [Serializable]
    public class ParserException : Exception
    {
        protected ParserException(int errorCode)
        {
        }

        public ParserException(int errorCode, string message, params object[] args)
            : base(message)
        {
        }

        protected ParserException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal ParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #region Codes constants inner class
        public static class Codes
        {
            public const int ValidationFailed = PublicErrorCodeBase + 1;
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

            public const int RequiredArgumentsDefinedAfterOptional = InternalErrorCodeBase - 1;

            private const int PublicErrorCodeBase = 0;
            private const int InternalErrorCodeBase = 0;
        }
        #endregion
    }
}