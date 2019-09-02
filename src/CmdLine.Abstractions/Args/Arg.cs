﻿#region --- License & Copyright Notice ---
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
using System.Diagnostics;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for command-line args, such as <see cref="Option" />, <see cref="Argument" />
    ///     and <see cref="Command" />.
    ///     <para />
    ///     This class adds support for metadata in addition to multiple names from the base class.
    /// </summary>
    public abstract class Arg : NamedObject, IMetadataObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, object> _metadata;

        protected Arg()
        {
        }

        protected Arg(IDictionary<string, bool> names)
            : base(names)
        {
        }

        /// <inheritdoc />
        object IMetadataObject.this[string name]
        {
            get => ((IMetadataObject)this).Get<object>(name);
            set => ((IMetadataObject)this).Set(name, value);
        }

        /// <inheritdoc />
        T IMetadataObject.Get<T>(string name)
        {
            if (_metadata is null)
                return default;
            return _metadata.TryGetValue(name, out var result) ? (T)result : default;
        }

        /// <inheritdoc />
        void IMetadataObject.Set<T>(string name, T value)
        {
            if (_metadata is null)
                _metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (_metadata.ContainsKey(name))
                _metadata[name] = value;
            else
                _metadata.Add(name, value);
        }
    }
}
