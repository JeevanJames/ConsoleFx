// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Restores the current directory to the directory that was current before the command was
    ///     executed.
    /// </summary>
    public sealed class PushDirectoryAttribute : PrePostHandlerAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _originalDirectory;

        public override void BeforeHandler(Command command)
        {
            _originalDirectory = Directory.GetCurrentDirectory();
        }

        public override void AfterHandler(Command command)
        {
            Directory.SetCurrentDirectory(_originalDirectory);
        }
    }
}
