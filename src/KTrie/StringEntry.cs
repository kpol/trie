namespace KTrie;

/// <summary>
/// Defines a key/value pair that can be set or retrieved from <see cref="StringTrie{TValue}"/>.
/// </summary>
public struct StringEntry<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringEntry{TValue}"/> structure with the specified key and value.
    /// </summary>
    /// <param name="key">The <see cref="string"/> object defined in each key/value pair.</param>
    /// <param name="value">The definition associated with key.</param>
    public StringEntry(string key, TValue value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets the key in the key/value pair.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the value in the key/value pair.
    /// </summary>
    public TValue Value { get; }
}