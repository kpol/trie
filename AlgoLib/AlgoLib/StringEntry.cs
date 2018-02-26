namespace AlgoLib
{
    /// <summary>
    /// Defines a key/value pair that can be set or retrieved from <see cref="Trie{TValue}"/>.
    /// </summary>
    public class StringEntry<TValue> : TrieEntry<char, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringEntry{TValue}"/> structure with the specified key and value.
        /// </summary>
        /// <param name="key">The <see cref="string"/> object defined in each key/value pair.</param>
        /// <param name="value">The definition associated with key.</param>
        public StringEntry(string key, TValue value) : base(key, value)
        {
        }
    }
}