// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine.Validators.Bases
{
    public interface IValidator
    {
        void Validate(string parameterValue);

        Type ExpectedType { get; }

        object Value { get; }
    }
}
