#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

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

namespace ConsoleFx.Parser
{
    /// <summary>
    ///     Base class for objects that store ad-hoc metadata.
    ///     Base class for <see cref="T:ConsoleFx.Parser.Option" /> and <see cref="T:ConsoleFx.Parser.Argument" />.
    /// </summary>
    public abstract partial class MetadataObject
    {
        private CustomMetadata _metadata;

        /// <summary>
        ///     Optional metadata that can be used by ancillary frameworks, such as the usage builders.
        ///     This is simply a key-value structure.
        /// </summary>
        public CustomMetadata Metadata => _metadata ?? (_metadata = new CustomMetadata());
    }

    public abstract partial class MetadataObject
    {
        public sealed class CustomMetadata
        {
            private Dictionary<string, object> _metadata;

            public string this[string name]
            {
                get { return Get<string>(name); }
                set { Set(name, value); }
            }

            public T Get<T>(string name)
            {
                if (_metadata == null)
                    return default(T);
                object result;
                return _metadata.TryGetValue(name, out result) ? (T)result : default(T);
            }

            public void Set<T>(string name, T value)
            {
                if (_metadata == null)
                    _metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                if (_metadata.ContainsKey(name))
                    _metadata[name] = value;
                else
                    _metadata.Add(name, value);
            }
        }
    }
}