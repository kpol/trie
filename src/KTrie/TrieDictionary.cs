using KTrie.TrieNodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KTrie;

public sealed class TrieDictionary<TValue>(IEqualityComparer<char>? comparer = null)
    : IDictionary<string, TValue>, IReadOnlyDictionary<string, TValue>
{
    private readonly Trie _trie = new(comparer);

    public ICollection<string> Keys => _trie.ToList();

    public ICollection<TValue> Values =>
        _trie.GetAllTerminalNodes().Select(t => ((TerminalValueCharTrieNode)t).Value).ToList();

    public int Count => _trie.Count;

    bool ICollection<KeyValuePair<string, TValue>>.IsReadOnly => false;

    IEnumerable<string> IReadOnlyDictionary<string, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<string, TValue>.Values => Values;

    public TValue this[string key]
    {
        get
        {
            if (!TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException();
            }

            return value;
        }
        set => TryAdd(key, value, true);
    }

    public void Clear() => _trie.Clear();

    public void Add(string key, TValue value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        if (!TryAdd(key, value))
        {
            throw new ArgumentException($"An item with the same key has already been added. Key: {key}", nameof(key));
        }
    }

    public bool TryAdd(string key, TValue value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        return TryAdd(key, value, false);
    }

    private bool TryAdd(string key, TValue value, bool overrideValue)
    {
        var terminalNode = _trie.AddNodesFromUpToBottom(key);

        if (terminalNode.existingTerminalNode is not null && terminalNode.existingTerminalNode.IsTerminal)
        {
            if (!overrideValue) return false;

            ((TerminalValueCharTrieNode)terminalNode.existingTerminalNode).Value = value;

            return true;
        }

        var newTerminalNode = new TerminalValueCharTrieNode(key[^1]) { Word = key, Value = value };

        _trie.AddTerminalNode(terminalNode.parent, terminalNode.existingTerminalNode, newTerminalNode, key);

        return true;
    }

    public IEnumerable<KeyValuePair<string, TValue>> StartsWith(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        return _();

        IEnumerable<KeyValuePair<string, TValue>> _() =>
            _trie.GetTerminalNodesByPrefix(value)
                .Select(t => new KeyValuePair<string, TValue>(t.Word, ((TerminalValueCharTrieNode)t).Value));
    }

    public IEnumerable<KeyValuePair<string, TValue>> Matches(IReadOnlyList<Character> pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfZero(pattern.Count);

        return _();

        IEnumerable<KeyValuePair<string, TValue>> _() =>
            _trie.GetNodesByPattern(pattern).Where(n => n.IsTerminal).Cast<TerminalValueCharTrieNode>()
                .Select(n => new KeyValuePair<string, TValue>(n.Word, n.Value));
    }

    public IEnumerable<KeyValuePair<string, TValue>> StartsWith(IReadOnlyList<Character> pattern)
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

                foreach (var terminalNode in Trie.GetDescendantTerminalNodes(n).Cast<TerminalValueCharTrieNode>())
                {
                    yield return new KeyValuePair<string, TValue>(terminalNode.Word, terminalNode.Value);
                }
            }
        }
    }

    public bool Remove(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        return _trie.Remove(key);
    }

    public bool ContainsKey(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        return _trie.Contains(key);
    }

    public bool TryGetValue(string key, out TValue value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var node = _trie.GetNode(key);

        if (node is not null && node.IsTerminal)
        {
            value = ((TerminalValueCharTrieNode)node).Value;

            return true;
        }

        value = default!;

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

    private sealed class TerminalValueCharTrieNode(char key) : TerminalCharTrieNode(key)
    {
        public TValue Value { get; set; } = default!;

        public override string ToString() => $"Key: {Key}, Word: {Word}, Value: {Value}";
    }
}