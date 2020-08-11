#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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

using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLine
{
    // INamedObject implementation
    public partial class Command : INamedObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly INamedObject _namedObject = new NamedObjectImpl();

        /// <inheritdoc />
        public string Name => _namedObject.Name;

        /// <inheritdoc />
        public IEnumerable<string> AlternateNames => _namedObject.AlternateNames;

        /// <inheritdoc />
        public IEnumerable<string> AllNames => _namedObject.AllNames;

        /// <inheritdoc />
        public void AddName(string name, bool caseSensitive = false)
        {
            _namedObject.AddName(name, caseSensitive);
        }

        /// <inheritdoc />
        public bool HasName(string name)
        {
            return _namedObject.HasName(name);
        }
    }
}
