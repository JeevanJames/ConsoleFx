﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;
using ConsoleFx.CmdLine.Validators;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.TestHarness.SimpleProgram
{
    [Program(Style = ArgStyle.Unix)]
    public sealed class SendEmailProgram : ConsoleProgram
    {
        public static async Task<int> Main()
        {
            ConsoleProgram program = new SendEmailProgram();
            program.HandleErrorsWith(ex =>
            {
                ConsoleEx.PrintLine($"[Red]{ex}[Reset]");
                return 1;
            });
            program.DisplayHelpOnError();
            program.ScanEntryAssemblyForCommands();
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
        public override string Validate(IParseResult parseResult)
        {
            if (parseResult.Group == 0)
            {
                if (ToAddresses.Count == 0 && CcAddresses.Count == 0 && BccAddresses.Count == 0)
                    return "Specify at least one To, CC or BCC address";
            }

            return null;
        }

        [Option("to", Optional = true, MultipleOccurrences = true)]
        [OptionHelp("To addresses.")]
        [Required]
        public IList<string> ToAddresses { get; } = new List<string>();

        [Option("cc", Optional = true, MultipleOccurrences = true)]
        [OptionHelp("CC addresses.")]
        public IList<string> CcAddresses { get; } = new List<string>();

        [Option("bcc", Optional = true, MultipleOccurrences = true)]
        [OptionHelp("BCC addresses.")]
        public IList<string> BccAddresses { get; } = new List<string>();

        [Option("subject", "sub", "s")]
        [OptionHelp("Email subject line")]
        public string Subject { get; set; }

        [Argument]
        [StringValidator(2, int.MaxValue)]
        [ArgumentHelp("message", "The email message")]
        public string Message { get; set; }
    }

    [Command("verify")]
    [CommandHelp("Verifies things")]
    public sealed class VerifyCommand : Command
    {
        /// <inheritdoc />
        protected override int HandleCommand()
        {
            Console.WriteLine("In verify");
            return 0;
        }
    }
}
