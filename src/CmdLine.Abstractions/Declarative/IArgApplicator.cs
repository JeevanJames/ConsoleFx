// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Applied to attributes decorated on args. Provides a common way to update the args from
    ///     the attribute.
    /// </summary>
    /// <typeparam name="TArg">The type of the arg.</typeparam>
    public interface IArgApplicator<in TArg>
        where TArg : ArgumentOrOption<TArg>
    {
        void Apply(TArg arg, PropertyInfo propertyInfo);
    }
}
