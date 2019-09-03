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

namespace ConsoleFx.ConsoleExtensions
{
    public sealed class ConsoleCapture
    {
        private Action<string> _outputHandler;

        private Action<string> _errorHandler;

        public ConsoleCapture(string program)
        {
            if (string.IsNullOrWhiteSpace(program))
                throw new ArgumentException("message", nameof(program));
            Program = program;
        }

        public ConsoleCapture(string program, params string[] args)
            : this(program)
        {
            if (args?.Length > 0)
                Args = string.Join(" ", args);
        }

        public string Program { get; }

        public string Args { get; }

        public Action<string> OutputHandler => _outputHandler;

        public Action<string> ErrorHandler => _errorHandler;

        public ConsoleCapture OnOutput(Action<string> handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));
            _outputHandler = handler;
            return this;
        }

        public ConsoleCapture OnError(Action<string> handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));
            _errorHandler = handler;
            return this;
        }

        public int Start()
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = Program;
                process.StartInfo.Arguments = Args;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.ErrorDialog = false;
                process.StartInfo.UseShellExecute = false;

                if (_outputHandler != null)
                {
                    process.StartInfo.RedirectStandardOutput = true;
                    process.OutputDataReceived += (sender, e) => _outputHandler(e.Data);
                }

                if (_errorHandler != null)
                {
                    process.StartInfo.RedirectStandardError = true;
                    process.ErrorDataReceived += (sender, e) => _errorHandler(e.Data);
                }

                process.Start();

                if (_outputHandler != null)
                    process.BeginOutputReadLine();
                if (_errorHandler != null)
                    process.BeginErrorReadLine();

                process.WaitForExit();

                return process.ExitCode;
            }
        }
    }
}
