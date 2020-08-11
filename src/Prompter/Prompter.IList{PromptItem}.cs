using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.Prompter
{
    public sealed partial class Prompter : IList<PromptItem>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<PromptItem> _promptItems = new List<PromptItem>();

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
