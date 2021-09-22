// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;

namespace ConsoleFx.TestHarness.SimpleProgram
{
#pragma warning disable CA2227 // Collection properties should be read only
    [Program(Name = "simple-program", Style = ArgStyle.Unix)]
    [Command("send-email")]
    public sealed class SendEmailProgram : ConsoleProgram
    {
        public static async Task<int> Main()
        {
            ConsoleProgram program = new SendEmailProgram()
                .DisplayHelpOnError();
#if DEBUG
            return await program.RunDebugAsync(condition: () => true).ConfigureAwait(false);
#else
            return await program.RunWithCommandLineArgsAsync().ConfigureAwait(false);
#endif
        }

        protected override int HandleCommand()
        {
            Console.WriteLine($"To     : {string.Join(',', ToAddresses)}");
            Console.WriteLine($"CC     : {string.Join(',', CcAddresses)}");
            Console.WriteLine($"BCC    : {string.Join(',', BccAddresses)}");
            Console.WriteLine($"Subject: {Subject}");
            Console.WriteLine($"Message: {Message}");
            return 0;
        }

        /// <inheritdoc />
        public override string Validate()
        {
            if (ToAddresses.Count == 0 && CcAddresses.Count == 0 && BccAddresses.Count == 0)
                return "Specify at least one To, CC or BCC address";
            return null;
        }

        [Option("to", Optional = true, Usage = CommonOptionUsage.UnlimitedOccurrencesSingleParameter)]
        [Help("To addresses.")]
        public IList<string> ToAddresses { get; set; } = new List<string>();

        [Option("cc", Optional = true, Usage = CommonOptionUsage.UnlimitedOccurrencesSingleParameter)]
        [Help("CC addresses.")]
        public IList<string> CcAddresses { get; set; } = new List<string>();

        [Option("bcc", Optional = true, Usage = CommonOptionUsage.UnlimitedOccurrencesSingleParameter)]
        [Help("BCC addresses.")]
        public IList<string> BccAddresses { get; set; } = new List<string>();

        [Option("subject", "sub", "s", Usage = CommonOptionUsage.SingleParameter)]
        [Help("Email subject line")]
        public string Subject { get; set; }

        [Argument]
        [StringValidator(2, int.MaxValue)]
        [Help("The email message")]
        public string Message { get; set; }
    }
}
#pragma warning restore CA2227 // Collection properties should be read only
