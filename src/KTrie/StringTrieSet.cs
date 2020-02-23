using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KTrie
{
    public class StringTrieSet : ICollection<string>
    {
        private readonly TrieSet<char> _trie;

        public StringTrieSet() : this(EqualityComparer<char>.Default)
        {
            
        }

        public StringTrieSet(IEqualityComparer<char> comparer)
        {
            _trie = new TrieSet<char>(comparer);
        }

        public int Count => _trie.Count;

        bool ICollection<string>.IsReadOnly => false;

        public IEnumerable<string> GetByPrefix(string prefix) =>
            _trie.GetByPrefix(prefix).Select(c => new string(c.ToArray()));

        public IEnumerator<string> GetEnumerator() => _trie.Select(c => new string(c.ToArray())).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(string item) => _trie.Add(item);

        public void AddRange(IEnumerable<string> item) => _trie.AddRange(item);

        public void Clear() => _trie.Clear();

        public bool Contains(string item) => _trie.Contains(item);

        public void CopyTo(string[] array, int arrayIndex) =>
            Array.Copy(_trie.Select(c => new string(c.ToArray())).ToArray(), 0, array, arrayIndex, Count);

        public bool Remove(string item) => _trie.Remove(item);
    }
}