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
using System.Diagnostics;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for command-line args, such as <see cref="Option" />, <see cref="Argument" />
    ///     and <see cref="Command" />.
    ///     <para />
    ///     This class adds support for metadata.
    /// </summary>
    public abstract class Arg : IMetadataObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<Dictionary<string, object>> _metadata = new Lazy<Dictionary<string, object>>(
            () => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

        /// <summary>
        ///     Initializes a new instance of the <see cref="Arg"/> class.
        /// </summary>
        protected Arg()
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
            return _metadata.Value.TryGetValue(name, out var result) ? (T)result : default;
        }

        /// <inheritdoc />
        void IMetadataObject.Set<T>(string name, T value)
        {
            if (_metadata.Value.ContainsKey(name))
                _metadata.Value[name] = value;
            else
                _metadata.Value.Add(name, value);
        }
    }
}
