// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
            using var process = new Process();
            process.StartInfo.FileName = Program;
            process.StartInfo.Arguments = Args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.UseShellExecute = false;

            if (_outputHandler != null)
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (_, e) => _outputHandler(e.Data);
            }

            if (_errorHandler != null)
            {
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += (_, e) => _errorHandler(e.Data);
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
