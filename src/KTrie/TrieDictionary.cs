using KTrie.TrieNodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace KTrie;

public sealed class TrieDictionary<TValue>(IEqualityComparer<char>? comparer = null)
    : IDictionary<string, TValue>, IReadOnlyDictionary<string, TValue>
{
    private readonly Trie _trie = new(comparer);

    public ICollection<string> Keys => [.. _trie];

    public ICollection<TValue> Values =>
        [.. _trie.GetAllTerminalNodes().Select(t => ((TerminalValueCharTrieNode)t).Value)];

    public int Count => _trie.Count;

    bool ICollection<KeyValuePair<string, TValue>>.IsReadOnly => false;

    IEnumerable<string> IReadOnlyDictionary<string, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<string, TValue>.Values => Values;

    public TValue this[string key]
    {
        get => this[key.AsSpan()];
        set => this[key.AsSpan()] = value;
    }

    public TValue this[ReadOnlySpan<char> key]
    {
        get
        {
            SpanException.ThrowIfNullOrEmpty(key);

            if (!TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException();
            }

            return value;
        }
        set
        {
            SpanException.ThrowIfNullOrEmpty(key);

            TryAdd(key, value, InsertionBehavior.OverwriteExisting);
        }
    }

    public void Clear() => _trie.Clear();

    public void Add(string key, TValue value) => Add(key.AsSpan(), value);

    public void Add(ReadOnlySpan<char> key, TValue value)
    {
        SpanException.ThrowIfNullOrEmpty(key);

        TryAdd(key, value, InsertionBehavior.ThrowOnExisting);
    }

    public bool TryAdd(string key, TValue value) => TryAdd(key.AsSpan(), value);

    public bool TryAdd(ReadOnlySpan<char> key, TValue value)
    {
        SpanException.ThrowIfNullOrEmpty(key);

        return TryAdd(key, value, InsertionBehavior.None);
    }

    public IEnumerable<KeyValuePair<string, TValue>> EnumerateByPrefix(string prefix) => EnumerateByPrefix(prefix.AsSpan());

    public IEnumerable<KeyValuePair<string, TValue>> EnumerateByPrefix(ReadOnlySpan<char> prefix)
    {
        SpanException.ThrowIfNullOrEmpty(prefix);

        var nodes = _trie.GetTerminalNodesByPrefix(prefix);

        return nodes.Select(t => new KeyValuePair<string, TValue>(t.Word, ((TerminalValueCharTrieNode)t).Value));
    }

    public IEnumerable<KeyValuePair<string, TValue>> EnumerateMatches(IReadOnlyList<Character> pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfZero(pattern.Count);

        return _trie.GetNodesByPattern(pattern)
            .Where(n => n.IsTerminal).Cast<TerminalValueCharTrieNode>()
            .Select(n => new KeyValuePair<string, TValue>(n.Word, n.Value));
    }

    public IEnumerable<KeyValuePair<string, TValue>> EnumerateByPrefix(IReadOnlyList<Character> pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfZero(pattern.Count);

        return _();

        IEnumerable<KeyValuePair<string, TValue>> _()
        {
            foreach (var n in _trie.GetNodesByPattern(pattern))
            {
                if (n.IsTerminal)
                {
                    var terminalNode = (TerminalValueCharTrieNode)n;

                    yield return new KeyValuePair<string, TValue>(terminalNode.Word, terminalNode.Value);
                }

                foreach (TerminalValueCharTrieNode terminalNode in TrieEnumerables.GetDescendantTerminalNodes(n))
                {
                    yield return new KeyValuePair<string, TValue>(terminalNode.Word, terminalNode.Value);
                }
            }
        }
    }

    public bool Remove(string key) => Remove(key.AsSpan());

    public bool Remove(ReadOnlySpan<char> key)
    {
        SpanException.ThrowIfNullOrEmpty(key);

        return _trie.Remove(key);
    }

    public KeyValuePair<string, TValue>? LongestPrefixMatch(string input) => LongestPrefixMatch(input.AsSpan());

    public KeyValuePair<string, TValue>? LongestPrefixMatch(ReadOnlySpan<char> input)
    {
        SpanException.ThrowIfNullOrEmpty(input);

        var node = (TerminalValueCharTrieNode?)_trie.GetTerminalCharTrieNodeLongestPrefixMatch(input);

        if (node is null) return null;

        return new KeyValuePair<string, TValue>(node.Word, node.Value);
    }

    public bool ContainsKey(string key) => ContainsKey(key.AsSpan());

    public bool ContainsKey(ReadOnlySpan<char> key)
    {
        SpanException.ThrowIfNullOrEmpty(key);

        return _trie.Contains(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value) => TryGetValue(key.AsSpan(), out value);

    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue value)
    {
        SpanException.ThrowIfNullOrEmpty(key);

        var node = _trie.GetNode(key);

        if (node is not null && node.IsTerminal)
        {
            value = ((TerminalValueCharTrieNode)node).Value;

            return true;
        }

        value = default;

        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() =>
        _trie.GetAllTerminalNodes()
            .Select(t => new KeyValuePair<string, TValue>(t.Word, ((TerminalValueCharTrieNode)t).Value)).GetEnumerator();

    bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item) =>
        TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);

    void ICollection<KeyValuePair<string, TValue>>.CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);

        if (Count > array.Length - arrayIndex)
        {
            throw new ArgumentException(
                "The number of elements in the trie is greater than the available space from index to the end of the destination array.");
        }

        foreach (var node in _trie.GetAllTerminalNodes().Cast<TerminalValueCharTrieNode>())
        {
            array[arrayIndex++] = new KeyValuePair<string, TValue>(node.Word, node.Value);
        }
    }

    void ICollection<KeyValuePair<string, TValue>>.Add(KeyValuePair<string, TValue> item) => Add(item.Key, item.Value);

    bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item)
    {
        if (TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
        {
            return Remove(item.Key);
        }

        return false;
    }

    private bool TryAdd(ReadOnlySpan<char> key, TValue value, InsertionBehavior insertionBehavior)
    {
        var (existingTerminalNode, parent) = _trie.AddNodesFromUpToBottom(key);

        if (existingTerminalNode is not null && existingTerminalNode.IsTerminal)
        {
            switch (insertionBehavior)
            {
                case InsertionBehavior.ThrowOnExisting:
                    throw new ArgumentException($"An item with the same key has already been added. Key: {key}", nameof(key));
                case InsertionBehavior.None:
                    return false;
                case InsertionBehavior.OverwriteExisting:
                default:
                    ((TerminalValueCharTrieNode)existingTerminalNode).Value = value;

                    return true;
            }
        }

        var newTerminalNode = new TerminalValueCharTrieNode(key[^1]) { Word = key.ToString(), Value = value };

        _trie.AddTerminalNode(parent, existingTerminalNode, newTerminalNode, key);

        return true;
    }

    private sealed class TerminalValueCharTrieNode(char key) : TerminalCharTrieNode(key)
    {
        public TValue Value { get; set; } = default!;

        public override string ToString() => $"Key: {Key}, Word: {Word}, Value: {Value}";
    }

    private enum InsertionBehavior : byte
    {
        None = 0,

        OverwriteExisting = 1,

        ThrowOnExisting = 2
    }
}