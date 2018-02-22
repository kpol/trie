using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AlgoLib
{
    /// <summary>
    /// Implementation of trie data structure.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the trie.</typeparam>
    /// <typeparam name="TValue">The type of values in the trie.</typeparam>
    public class TrieDictionary<TKey, TValue> : IDictionary<IEnumerable<TKey>, TValue>
    {
        private readonly TrieSet<TKey> _trie;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrieDictionary{TKey, TValue}"/>.
        /// </summary>
        public TrieDictionary() : this(EqualityComparer<TKey>.Default)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrieDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="comparer">Comparer.</param>
        public TrieDictionary(IEqualityComparer<TKey> comparer)
        {
            _trie = new TrieSet<TKey>(comparer);
        }

        /// <inheritdoc />
        public int Count => _trie.Count;

        /// <inheritdoc />
        bool ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.IsReadOnly => false;

        public ICollection<IEnumerable<TKey>> Keys => _trie.ToList();

        /// <inheritdoc />
        public ICollection<TValue> Values => _trie.Cast<TrieEntryPrivate>().Select(te => te.Value).ToArray();

        /// <inheritdoc />
        public TValue this[IEnumerable<TKey> key]
        {
            get
            {
                TValue val;

                if (TryGetValue(key, out val))
                {
                    return val;
                }

                throw new KeyNotFoundException($"The given key was not present in the trie.");
            }
            set
            {
                IEnumerable<TKey> trieEntry;
                // ReSharper disable once PossibleMultipleEnumeration
                var result = _trie.TryGetItem(key, out trieEntry);

                if (result)
                {
                    ((TrieEntryPrivate)trieEntry).Value = value;
                }
                else
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    Add(key, value);
                }
            }
        }

        /// <summary>
        /// Gets items by key prefix.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <returns>Collection of <see cref="TrieEntry{TKey, TValue}"/> items which have key which starts from specified <see cref="prefix"/>.</returns>
        public IEnumerable<TrieEntry<TKey, TValue>> GetByPrefix(IEnumerable<TKey> prefix)
        {
            return _trie.GetByPrefix(prefix).Cast<TrieEntryPrivate>().Select(i => new TrieEntry<TKey, TValue>(i, i.Value));
        }


        /// <inheritdoc />
        public IEnumerator<KeyValuePair<IEnumerable<TKey>, TValue>> GetEnumerator()
        {
            return _trie.Cast<TrieEntryPrivate>().Select(i => new KeyValuePair<IEnumerable<TKey>, TValue>(i, i.Value)).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.Add(KeyValuePair<IEnumerable<TKey>, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _trie.Clear();
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.Contains(KeyValuePair<IEnumerable<TKey>, TValue> item)
        {
            IEnumerable<TKey> trieEntry;
            var result = _trie.TryGetItem(item.Key, out trieEntry);

            if (result)
            {
                var value = ((TrieEntryPrivate)trieEntry).Value;

                if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.CopyTo(KeyValuePair<IEnumerable<TKey>, TValue>[] array, int arrayIndex)
        {
            Array.Copy(_trie.Cast<TrieEntryPrivate>().Select(i=>new KeyValuePair<IEnumerable<TKey>, TValue>(i, i.Value)).ToArray(), 0, array, arrayIndex, Count);
        }

        /// <inheritdoc />
        bool ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.Remove(KeyValuePair<IEnumerable<TKey>, TValue> item)
        {
            IEnumerable<TKey> trieEntry;
            var result = _trie.TryGetItem(item.Key, out trieEntry);

            if (result)
            {
                var value = ((TrieEntryPrivate)trieEntry).Value;

                if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
                {
                    return Remove(item.Key);
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool ContainsKey(IEnumerable<TKey> key)
        {
            return _trie.Contains(key);
        }

        /// <inheritdoc />
        public void Add(IEnumerable<TKey> key, TValue value)
        {
            _trie.Add(new TrieEntryPrivate(key) { Value = value });
        }

        /// <inheritdoc />
        public bool Remove(IEnumerable<TKey> key)
        {
            return  _trie.Remove(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(IEnumerable<TKey> key, out TValue value)
        {
            IEnumerable<TKey> trieEntry;
            var result = _trie.TryGetItem(key, out trieEntry);

            value = result ? ((TrieEntryPrivate)trieEntry).Value : default(TValue);

            return result;
        }

        private class TrieEntryPrivate : IEnumerable<TKey>
        {
            public TrieEntryPrivate(IEnumerable<TKey> key)
            {
                Key = key;
            }

            private IEnumerable<TKey> Key { get; }

            public TValue Value { get; set; }

            public IEnumerator<TKey> GetEnumerator()
            {
                return Key.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}