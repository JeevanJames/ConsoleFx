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
using System.Collections.Generic;

using ConsoleFx.ConsoleExtensions;

namespace TestHarness.ProgressBarTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            ProgressBar simple = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "Default progress bar [<<bar>>] <<value>>/<<max>>"
            }, style: ProgressBarStyle.Default);

            ConsoleEx.PrintBlank();

            ProgressBar dots = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "Dots progress bar [<<bar>>] <<percentage>> | <<status>>"
            }, style: ProgressBarStyle.Dots.CompleteForeColor(CColor.Green));

            ConsoleEx.PrintBlank();

            ProgressBar blocks = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "<<percentage>>% :: <<status>> :: Blocks progress bar [<<bar>>]"
            }, style: ProgressBarStyle.Block.CompleteForeColor(CColor.Green).IncompleteBackColor(CColor.Red));

            ConsoleEx.PrintBlank();

            ProgressBar lines = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "Lines progress bar [<<bar>>] -- <<status>>"
            }, style: ProgressBarStyle.Lines
                .CompleteForeColor(CColor.Green)
                .CompleteBackColor(CColor.White)
                .IncompleteBackColor(CColor.White));

            var bars = new List<ProgressBar> { simple, dots, blocks, lines };

            ConsoleEx.WaitForKeysLoop(new[]
            {
                ConsoleKey.RightArrow.HandledBy(k => bars.ForEach(pb => pb.Value++)),
                ConsoleKey.LeftArrow.HandledBy(k => bars.ForEach(pb => pb.Value--)),
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
        }
    }
}
