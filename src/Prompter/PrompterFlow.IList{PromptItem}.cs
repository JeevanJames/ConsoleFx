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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.Prompter
{
    public sealed partial class PrompterFlow : IList<PromptItem>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<PromptItem> _promptItems = new();

        public PromptItem this[int index]
        {
            get => _promptItems[index];
            set => _promptItems[index] = value;
        }

        public int Count => _promptItems.Count;

        public bool IsReadOnly => ((IList<PromptItem>)_promptItems).IsReadOnly;

        public void Add(PromptItem item)
        {
            _promptItems.Add(item);
        }

        public void Clear()
        {
            _promptItems.Clear();
        }

        public bool Contains(PromptItem item)
        {
            return _promptItems.Contains(item);
        }

        public void CopyTo(PromptItem[] array, int arrayIndex)
        {
            _promptItems.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PromptItem> GetEnumerator()
        {
            return ((IList<PromptItem>)_promptItems).GetEnumerator();
        }

        public int IndexOf(PromptItem item)
        {
            return _promptItems.IndexOf(item);
        }

        public void Insert(int index, PromptItem item)
        {
            _promptItems.Insert(index, item);
        }

        public bool Remove(PromptItem item)
        {
            return _promptItems.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _promptItems.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<PromptItem>)_promptItems).GetEnumerator();
        }
    }
}
