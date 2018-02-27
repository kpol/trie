using System.Collections.Generic;

namespace AlgoLib
{
    /// <summary>
    /// Defines a key/value pair that can be set or retrieved from <see cref="Trie{TValue}"/>.
    /// </summary>
    public struct TrieEntry<TKey, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringEntry{TValue}"/> structure with the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The definition associated with key.</param>
        public TrieEntry(IEnumerable<TKey> key, TValue value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets the key in the key/value pair.
        /// </summary>
        public IEnumerable<TKey> Key { get; }

        /// <summary>
        /// Gets the value in the key/value pair.
        /// </summary>
        public TValue Value { get; }
    }
}
