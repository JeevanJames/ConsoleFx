// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        public static ProgressBar ProgressBar(ProgressBarSpec spec = null, int value = 0, ProgressBarStyle style = null)
        {
            return new ProgressBar(spec, value, style);
        }
    }
}
