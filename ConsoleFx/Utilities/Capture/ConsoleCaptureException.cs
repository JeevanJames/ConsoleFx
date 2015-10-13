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

namespace ConsoleFx.Utilities.Capture
{
    public sealed class ConsoleCaptureException : ConsoleFxException
    {
        public ConsoleCaptureException(int errorCode, string message)
            : base(errorCode, message)
        {
        }

        public ConsoleCaptureException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
        }

        public static class Codes
        {
            public const int ProcessStartFailed = 101;
            public const int ProcessAborted = 102;
        }
    }
}