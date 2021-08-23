// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine.Program
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ProgramAttribute : Attribute
    {
        public string Name { get; set; }

        public ArgStyle Style { get; set; } = ArgStyle.Unix;

        public ArgGrouping Grouping { get; set; } = ArgGrouping.DoesNotMatter;
    }
}
