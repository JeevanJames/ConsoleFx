// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;

using ConsoleFx.ConsoleExtensions;

namespace TestHarness.ProgressBarTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            ProgressBar simple = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "Default progress bar [<<bar>>] <<value>>/<<max>>",
            }, style: ProgressBarStyle.Default);

            ConsoleEx.PrintBlank();

            ProgressBar dots = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "Dots progress bar [<<bar>>] <<percentage>> | <<status>>",
            }, style: ProgressBarStyle.Dots.CompleteForeColor(CColor.Green));

            ConsoleEx.PrintBlank();

            ProgressBar blocks = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "<<percentage>>% :: <<status>> :: Blocks progress bar [<<bar>>]",
            }, style: ProgressBarStyle.Block.CompleteForeColor(CColor.Green).IncompleteBackColor(CColor.Red));

            ConsoleEx.PrintBlank();

            ProgressBar lines = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "Lines progress bar [<<bar>>] -- <<status>>",
            }, style: ProgressBarStyle.Lines
                .CompleteForeColor(CColor.Green)
                .CompleteBackColor(CColor.White)
                .IncompleteBackColor(CColor.White));

            var bars = new List<ProgressBar> { simple, dots, blocks, lines };

            var statusLine = ConsoleEx.StatusLine();

            ConsoleEx.WaitForKeysLoop(new[]
            {
                ConsoleKey.RightArrow.HandledBy(k => bars.ForEach(pb => pb.Value++)),
                ConsoleKey.LeftArrow.HandledBy(k => bars.ForEach(pb => pb.Value--)),
                ConsoleKey.UpArrow.HandledBy(k => statusLine.Status = DateTime.Now.ToString(CultureInfo.InvariantCulture)),
            }, postKeyPress: _ =>
            {
                string status;
                if (bars[0].Value < 20)
                    status = "Initializing";
                else if (bars[0].Value < 60)
                    status = "Executing";
                else if (bars[0].Value < 80)
                    status = "Cleaning up";
                else
                    status = "Shut down";
                bars.ForEach(pb => pb.Status = status);
            });

            statusLine.Dispose();
        }
    }
}
