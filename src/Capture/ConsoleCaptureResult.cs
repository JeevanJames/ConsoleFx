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
using System.Diagnostics;

namespace ConsoleFx.Capture
{
    /// <summary>
    ///     Represents the result of a console capture.
    /// </summary>
    [DebuggerDisplay("Exit code: {ExitCode}")]
    [Serializable]
    public sealed class ConsoleCaptureResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleCaptureResult"/> class.
        /// </summary>
        /// <param name="exitCode">The exit code from the process.</param>
        /// <param name="outputMessage">The text captured from standard output.</param>
        /// <param name="errorMessage">The text captured from standard error.</param>
        internal ConsoleCaptureResult(int exitCode, string outputMessage, string errorMessage)
        {
            ExitCode = exitCode;
            OutputMessage = outputMessage;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Gets the exit code from the executed process.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        ///     Gets the text captured from standard error.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        ///     Gets the text captured from standard output.
        /// </summary>
        public string OutputMessage { get; }
    }
}
