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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ConsoleFx.Utilities.Capture
{
    public sealed class ConsoleCapture
    {
        private string FileName { get; }
        private string Arguments { get; }

        public ConsoleCapture(string fileName)
            : this(fileName, null)
        {
        }

        public ConsoleCapture(string fileName, string arguments)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName))
                throw new ArgumentException($"File {fileName} does not exist.", nameof(fileName));
            FileName = fileName;
            Arguments = arguments;
        }

        /// <summary>
        /// Starts the specified application as a console app and captures the output and
        /// (optionally) the error output.
        /// </summary>
        /// <param name="captureError">Indicates whether to capture errors from the app.</param>
        /// <returns>An instance of <see cref="ConsoleCaptureResult"/></returns>
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
                    throw new ConsoleCaptureException(ConsoleCaptureException.Codes.ProcessStartFailed,
                        $"Could not start the process with file name {FileName}.");

                string outputMessage, errorMessage = string.Empty;
                int exitCode = captureError
                    ? CaptureOutputAndError(process, out outputMessage, out errorMessage) : CaptureOutput(process, out outputMessage);
                return new ConsoleCaptureResult(exitCode, outputMessage, errorMessage);
            }
        }

        /// <summary>
        /// Used to capture output if error output is not required
        /// </summary>
        /// <param name="process"></param>
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
        /// <param name="process"></param>
        /// <param name="outputMessage">The output message captured from the process.</param>
        /// <param name="errorMessage">The error message captured from the process.</param>
        /// <returns>The process exit code.</returns>
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
            } else
            {
                var waitHandles = new[] { outputResult.AsyncWaitHandle, errorResult.AsyncWaitHandle };
                if (!WaitHandle.WaitAll(waitHandles))
                    throw new ConsoleCaptureException(ConsoleCaptureException.Codes.ProcessAborted, "The process was aborted.");
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