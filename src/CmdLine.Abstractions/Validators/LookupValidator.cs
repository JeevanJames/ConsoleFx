// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public sealed class LookupValidator : SingleMessageValidator
    {
        private readonly List<string> _items;

        public LookupValidator(params string[] items)
            : this((IEnumerable<string>)items)
        {
        }

        public LookupValidator(IEnumerable<string> items)
            : base(Messages.Lookup)
        {
            _items = items != null ? new List<string>(items) : new List<string>();
        }

        public void Add(string item)
        {
            _items.Add(item);
        }

        public void AddRange(IEnumerable<string> items)
        {
            _items.AddRange(items);
        }

        public bool CaseSensitive { get; set; }

        protected override object ValidateAsString(string parameterValue)
        {
            StringComparison comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            if (!_items.Any(item => parameterValue.Equals(item, comparison)))
                ValidationFailed(parameterValue, Message);
            return parameterValue;
        }
    }

    public static class LookupValidatorExtensions
    {
        public static Argument ValidateFromList(this Argument argument, params string[] items) =>
            argument.ValidateWith(new LookupValidator(items));

        public static Argument ValidateFromList(this Argument argument, IEnumerable<string> items,
            bool caseSensitive = false, string message = null)
        {
            var validator = new LookupValidator(items);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateFromList(this Option option, params string[] items) =>
            option.ValidateWith(new LookupValidator(items));

        public static Option ValidateFromList(this Option option, IEnumerable<string> items, bool caseSensitive = false,
            int parameterIndex = -1, string message = null)
        {
            var validator = new LookupValidator(items);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(validator);
        }
    }
}
