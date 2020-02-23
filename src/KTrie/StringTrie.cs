using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KTrie
{
    /// <summary>
    /// Implementation of trie data structure.
    /// </summary>
    /// <typeparam name="TValue">The type of values in the trie.</typeparam>
    public class StringTrie<TValue> : IDictionary<string, TValue>
    {
        private readonly Trie<char, TValue> _trie;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTrie{TValue}"/>.
        /// </summary>
        /// <param name="comparer">Comparer.</param>
        public StringTrie(IEqualityComparer<char> comparer)
        {
            _trie = new Trie<char, TValue>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTrie{TValue}"/>.
        /// </summary>
        public StringTrie() : this(EqualityComparer<char>.Default)
        {
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count => _trie.Count;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<string> Keys => _trie.Keys.Select(i => new string(i.ToArray())).ToArray();

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TValue> Values => _trie.Values.ToArray();

        bool ICollection<KeyValuePair<string, TValue>>.IsReadOnly => false;

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception>
        public TValue this[string key]
        {
            get => _trie[key];

            set => _trie[key] = value;
        }

        /// <summary>
        /// Adds an element with the provided charKey and value to the <see cref="StringTrie{TValue}"/>.
        /// </summary>
        /// <param name="key">The object to use as the charKey of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same charKey already exists in the <see cref="StringTrie{TValue}"/>.</exception>
        public void Add(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            _trie.Add(key, value);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="StringTrie{TValue}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the <see cref="StringTrie{TValue}"/>. The items should have unique keys.</param>
        /// <exception cref="T:System.ArgumentException">An element with the same charKey already exists in the <see cref="StringTrie{TValue}"/>.</exception>
        public void AddRange(IEnumerable<StringEntry<TValue>> collection)
        {
            foreach (var item in collection)
            {
                _trie.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear() => _trie.Clear();

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified charKey.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the charKey; otherwise, false.
        /// </returns>
        /// <param name="key">The charKey to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(string key) => _trie.ContainsKey(key);

        /// <summary>
        /// Gets items by key prefix.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <returns>Collection of <see cref="StringEntry{TValue}"/> items which have key with specified key.</returns>
        public IEnumerable<StringEntry<TValue>> GetByPrefix(string prefix) => 
            _trie.GetByPrefix(prefix).Select(i => new StringEntry<TValue>(new string(i.Key.ToArray()), i.Value));

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => 
            _trie.Select(i => new KeyValuePair<string, TValue>(new string(i.Key.ToArray()), i.Value)).GetEnumerator();

        /// <summary>
        /// Removes the element with the specified charKey from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The charKey of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public bool Remove(string key) => _trie.Remove(key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue(string key, out TValue value) => _trie.TryGetValue(key, out value);

        void ICollection<KeyValuePair<string, TValue>>.Add(KeyValuePair<string, TValue> item) => 
            Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item) => 
            ((IDictionary<IEnumerable<char>, TValue>)_trie).Contains(new KeyValuePair<IEnumerable<char>, TValue>(item.Key, item.Value));

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex) => 
            Array.Copy(_trie.Select(i => new KeyValuePair<string, TValue>(new string(i.Key.ToArray()), i.Value)).ToArray(), 0, array, arrayIndex, Count);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item) => 
            ((IDictionary<IEnumerable<char>, TValue>)_trie).Remove(new KeyValuePair<IEnumerable<char>, TValue>(item.Key, item.Value));
    }
}