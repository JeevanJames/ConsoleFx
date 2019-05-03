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
using System.Threading;

namespace ConsoleFx.Capture
{
    /// <summary>
    ///     Executes a command-line application and captures its output.
    /// </summary>
    public sealed class ConsoleCapture
    {
        /// <summary>
        ///     Gets the path to the command-line application to execute.
        ///     <para/>
        ///     This could be an absolute path, relative path or a file name on the system path.
        /// </summary>
        private string FileName { get; }

        /// <summary>
        ///     Gets the arguments to pass to the command-line application to execute.
        /// </summary>
        private string Arguments { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleCapture"/> class with the
        ///     <paramref name="fileName"/> of the command-line application to execute.
        /// </summary>
        /// <param name="fileName">The file name of the command-line application to execute.</param>
        public ConsoleCapture(string fileName)
            : this(fileName, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleCapture"/> class with the
        ///     <paramref name="fileName"/> of the command-line application to execute and any
        ///     <paramref name="arguments"/> to pass to it.
        /// </summary>
        /// <param name="fileName">The file name of the command-line application to execute.</param>
        /// <param name="arguments">The arguments to pass to the command-line application.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="fileName"/> is <c>null</c>.</exception>
        public ConsoleCapture(string fileName, string arguments)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            FileName = fileName;
            Arguments = arguments;
        }

        /// <summary>
        ///     Starts the specified application as a console app and captures the output and (optionally)
        ///     the error output.
        /// </summary>
        /// <param name="captureError">Indicates whether to capture errors from the app.</param>
        /// <returns>An instance of <see cref="ConsoleCaptureResult"/></returns>
        /// <exception cref="ConsoleCaptureException">Thrown if the process cannot be started.</exception>
        public ConsoleCaptureResult Start(bool captureError = false)
        {
            using (var process = new Process())
            {
                //Need to enforce a command-line app as best we can
                process.StartInfo.FileName = FileName;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.ErrorDialog = false;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                if (captureError)
                    process.StartInfo.RedirectStandardError = true;

                if (!process.Start())
                {
                    throw new ConsoleCaptureException(ConsoleCaptureException.Codes.ProcessStartFailed,
                        string.Format(ErrorMessages.ProcessStartFailed, FileName));
                }

                string errorMessage = string.Empty;
                int exitCode = captureError
                    ? CaptureOutputAndError(process, out string outputMessage, out errorMessage)
                    : CaptureOutput(process, out outputMessage);
                return new ConsoleCaptureResult(exitCode, outputMessage, errorMessage);
            }
        }

        /// <summary>
        ///     Used to capture output if error output is not required
        /// </summary>
        /// <param name="process">The process to capture output from.</param>
        /// <param name="outputMessage">The output message captured from the process.</param>
        /// <returns>The process exit code.</returns>
        private static int CaptureOutput(Process process, out string outputMessage)
        {
            outputMessage = process.StandardOutput.ReadToEnd();

            if (!process.HasExited)
                process.WaitForExit();

            return process.ExitCode;
        }

        /// <summary>
        /// Used to capture both output and error output. This code is more complicated and is
        /// based on the CodeProject article by Andrew Tweddle:
        /// http://www.codeproject.com/KB/string/CommandLineHelper.aspx
        /// </summary>
        /// <param name="process">The process to capture output from.</param>
        /// <param name="outputMessage">The output message captured from the process.</param>
        /// <param name="errorMessage">The error message captured from the process.</param>
        /// <returns>The process exit code.</returns>
        /// <exception cref="ConsoleCaptureException">Thrown if any of processes fails.</exception>
        private static int CaptureOutputAndError(Process process, out string outputMessage, out string errorMessage)
        {
            Func<string> outputReader = process.StandardOutput.ReadToEnd;
            Func<string> errorReader = process.StandardError.ReadToEnd;

            IAsyncResult outputResult = outputReader.BeginInvoke(null, null);
            IAsyncResult errorResult = errorReader.BeginInvoke(null, null);

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                //WaitHandle.WaitAll fails on single-threaded apartments. Poll for completion instead.
                while (!(outputResult.IsCompleted && errorResult.IsCompleted))
                    Thread.Sleep(100);
            }
            else
            {
                WaitHandle[] waitHandles = new[] { outputResult.AsyncWaitHandle, errorResult.AsyncWaitHandle };
                if (!WaitHandle.WaitAll(waitHandles))
                {
                    throw new ConsoleCaptureException(ConsoleCaptureException.Codes.ProcessAborted,
                        ErrorMessages.ProcessAborted);
                }
            }

            outputMessage = outputReader.EndInvoke(outputResult);
            errorMessage = errorReader.EndInvoke(errorResult);

            if (!process.HasExited)
                process.WaitForExit();

            return process.ExitCode;
        }

        //Static shortcut for capturing console output.
        public static ConsoleCaptureResult Start(string fileName, string arguments = null, bool captureError = false)
        {
            return new ConsoleCapture(fileName, arguments).Start(captureError);
        }
    }
}
