// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.Prompter
{
    public sealed class BetweenPromptEventArgs : EventArgs
    {
        public PromptItem PreviousPrompt { get; set; }

        public PromptItem NextPrompt { get; set; }
    }
}
