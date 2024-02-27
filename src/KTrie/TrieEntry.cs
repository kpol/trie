using System.Collections.Generic;

namespace KTrie;

/// <summary>
/// Defines a key/value pair that can be set or retrieved from <see cref="StringTrie{TValue}"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StringEntry{TValue}"/> structure with the specified key and value.
/// </remarks>
/// <param name="key">The key.</param>
/// <param name="value">The definition associated with key.</param>
public readonly struct TrieEntry<TKey, TValue>(IEnumerable<TKey> key, TValue value)
{

    /// <summary>
    /// Gets the key in the key/value pair.
    /// </summary>
    public IEnumerable<TKey> Key { get; } = key;

    /// <summary>
    /// Gets the value in the key/value pair.
    /// </summary>
    public TValue Value { get; } = value;
}