using KTrie.TrieNodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KTrie;

public sealed class Trie : ICollection<string>, IReadOnlyCollection<string>
{
    private readonly IEqualityComparer<char> _comparer;

    private readonly CharTrieNode _root = new(char.MinValue);

    public Trie(IEqualityComparer<char>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<char>.Default;
    }

    public int Count { get; private set; }

    bool ICollection<string>.IsReadOnly => false;

    public bool Add(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        return Add(word.AsSpan());
    }

    public bool Add(ReadOnlySpan<char> word)
    {
        SpanException.ThrowIfNullOrEmpty(word);

        var (existingTerminalNode, parent) = AddNodesFromUpToBottom(word);

        if (existingTerminalNode is not null && existingTerminalNode.IsTerminal) return false; // already exists

        var newTerminalNode = new TerminalCharTrieNode(word[^1]) { Word = word.ToString() };

        AddTerminalNode(parent, existingTerminalNode, newTerminalNode, word);

        return true;
    }

    public void Clear()
    {
        _root.Children = [];
        Count = 0;
    }

    public bool Contains(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        return Contains(word.AsSpan());
    }

    public bool Contains(ReadOnlySpan<char> word)
    {
        SpanException.ThrowIfNullOrEmpty(word);

        var node = GetNode(word);

        return node is not null && node.IsTerminal;
    }

    public bool Remove(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        return Remove(word.AsSpan());
    }

    public bool Remove(ReadOnlySpan<char> word)
    {
        SpanException.ThrowIfNullOrEmpty(word);

        var nodesUpToBottom = GetNodesForRemoval(word);

        if (nodesUpToBottom.Count == 0) return false;

        RemoveNode(nodesUpToBottom);

        return true;
    }

    public string? LongestPrefixMatch(string input) => LongestPrefixMatch(input.AsSpan());

    public string? LongestPrefixMatch(ReadOnlySpan<char> input)
    {
        SpanException.ThrowIfNullOrEmpty(input);

        var longest = GetTerminalCharTrieNodeLongestPrefixMatch(input);

        return longest?.Word;
    }

    public IEnumerable<string> EnumerateByPrefix(string prefix) => EnumerateByPrefix(prefix.AsSpan());

    public IEnumerable<string> EnumerateByPrefix(ReadOnlySpan<char> prefix)
    {
        SpanException.ThrowIfNullOrEmpty(prefix);

        return GetTerminalNodesByPrefix(prefix).Select(n => n.Word);
    }

    public IEnumerable<string> EnumerateMatches(IReadOnlyList<Character> pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfZero(pattern.Count);

        return GetNodesByPattern(pattern)
                .Where(n => n.IsTerminal)
                .Cast<TerminalCharTrieNode>()
                .Select(n => n.Word);
    }

    public IEnumerable<string> EnumerateByPrefix(IReadOnlyList<Character> pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfZero(pattern.Count);

        return _();

        IEnumerable<string> _()
        {
            foreach (var n in GetNodesByPattern(pattern))
            {
                if (n.IsTerminal)
                {
                    yield return ((TerminalCharTrieNode)n).Word;
                }

                foreach (var terminalNode in GetDescendantTerminalNodes(n))
                {
                    yield return terminalNode.Word;
                }
            }
        }
    }

    internal (CharTrieNode? existingTerminalNode, CharTrieNode parent) AddNodesFromUpToBottom(ReadOnlySpan<char> word)
    {
        var current = _root;

        for (int i = 0; i < word.Length - 1; i++)
        {
            var n = GetChildNode(current, word[i]);

            if (n is not null)
            {
                current = n;
            }
            else
            {
                CharTrieNode node = new(word[i]);
                AddToNode(current, node);
                current = node;
            }
        }

        var terminalNode = GetChildNode(current, word[^1]);

        return (terminalNode, current);
    }

    internal void AddTerminalNode(CharTrieNode parent, CharTrieNode? existingNode, CharTrieNode newTerminalNode, ReadOnlySpan<char> word)
    {
        if (existingNode is not null)
        {
            newTerminalNode.CopyChildren(existingNode.Children);

            RemoveChildFromNode(parent, word[^1]);
        }

        AddToNode(parent, newTerminalNode);
        Count++;
    }

    internal IEnumerable<TerminalCharTrieNode> GetTerminalNodesByPrefix(ReadOnlySpan<char> prefix)
    {
        var node = GetNode(prefix);
        return GetTerminalNodes(node);
    }

    private static IEnumerable<TerminalCharTrieNode> GetTerminalNodes(CharTrieNode? node)
    {
        if (node is null)
        {
            yield break;
        }

        if (node.IsTerminal)
        {
            yield return (TerminalCharTrieNode)node;
        }

        foreach (var n in GetDescendantTerminalNodes(node))
        {
            yield return n;
        }
    }

    public IEnumerator<string> GetEnumerator() => GetAllTerminalNodes().Select(n => n.Word).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void ICollection<string>.Add(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        Add(word);
    }

    void ICollection<string>.CopyTo(string[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);

        if (Count > array.Length - arrayIndex)
        {
            throw new ArgumentException(
                "The number of elements in the trie is greater than the available space from index to the end of the destination array.");
        }

        foreach (var node in GetAllTerminalNodes())
        {
            array[arrayIndex++] = node.Word;
        }
    }

    internal IEnumerable<TerminalCharTrieNode> GetAllTerminalNodes() => GetDescendantTerminalNodes(_root);

    internal static IEnumerable<TerminalCharTrieNode> GetDescendantTerminalNodes(CharTrieNode node)
    {
        Queue<CharTrieNode> queue = new(node.Children);

        while (queue.Count > 0)
        {
            var n = queue.Dequeue();

            if (n.IsTerminal)
            {
                yield return (TerminalCharTrieNode)n;
            }

            for (var i = 0; i < n.Children.Length; i++)
            {
                queue.Enqueue(n.Children[i]);
            }
        }
    }

    internal CharTrieNode? GetNode(ReadOnlySpan<char> prefix)
    {
        var current = _root;

        for (var i = 0; i < prefix.Length; i++)
        {
            current = GetChildNode(current, prefix[i]);

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    internal IEnumerable<CharTrieNode> GetNodesByPattern(IReadOnlyList<Character> pattern)
    {
        Queue<(CharTrieNode node, int index)> queue = [];
        queue.Enqueue((_root, 0));

        while (queue.Count > 0)
        {
            var (node, index) = queue.Dequeue();

            if (index == pattern.Count - 1)
            {
                if (pattern[index].Char is { } ch)
                {
                    var n = GetChildNode(node, ch);

                    if (n is not null)
                    {
                        yield return n;
                    }
                }
                else
                {
                    for (var i = 0; i < node.Children.Length; i++)
                    {
                        yield return node.Children[i];
                    }
                }
            }
            else
            {
                if (pattern[index].Char is { } ch)
                {
                    var n = GetChildNode(node, ch);

                    if (n is not null)
                    {
                        queue.Enqueue((n, index + 1));
                    }
                }
                else
                {
                    for (var i = 0; i < node.Children.Length; i++)
                    {
                        queue.Enqueue((node.Children[i], index + 1));
                    }
                }
            }
        }
    }

    internal TerminalCharTrieNode? GetTerminalCharTrieNodeLongestPrefixMatch(ReadOnlySpan<char> input)
    {
        var node = _root;
        TerminalCharTrieNode? longest = null;

        foreach (char c in input)
        {
            node = GetChildNode(node, c);

            if (node is null)
            {
                break;
            }

            if (node.IsTerminal)
            {
                longest = (TerminalCharTrieNode)node;
            }
        }

        return longest;
    }

    private Stack<CharTrieNode> GetNodesForRemoval(ReadOnlySpan<char> prefix)
    {
        var current = _root;

        Stack<CharTrieNode> nodesUpToBottom = [];
        nodesUpToBottom.Push(_root);

        for (var i = 0; i < prefix.Length; i++)
        {
            var c = prefix[i];
            current = GetChildNode(current, c);

            if (current is not null)
            {
                nodesUpToBottom.Push(current);
            }
            else
            {
                return [];
            }
        }

        return current.IsTerminal ? nodesUpToBottom : [];
    }

    private void RemoveNode(Stack<CharTrieNode> nodesUpToBottom)
    {
        Count--;

        var node = nodesUpToBottom.Pop();

        if (node.Children.Length == 0)
        {
            while (node.Children.Length == 0 && nodesUpToBottom.Count > 0)
            {
                var parent = nodesUpToBottom.Pop();
                RemoveChildFromNode(parent, node.Key);

                if (parent.IsTerminal) return;

                node = parent;

            }
        }
        else
        {
            // convert node to non-terminal node
            CharTrieNode n = new(node.Key);
            n.CopyChildren(node.Children);

            var parent = nodesUpToBottom.Count == 0 ? _root : nodesUpToBottom.Pop();

            RemoveChildFromNode(parent, node.Key);
            AddToNode(parent, n);
        }
    }

    private void AddToNode(CharTrieNode node, CharTrieNode nodeToAdd)
    {
        for (var i = 0; i < node.Children.Length; i++)
        {
            if (_comparer.Equals(nodeToAdd.Key, node.Children[i].Key))
            {
                return;
            }
        }

        node.AddChild(nodeToAdd);
    }

    private void RemoveChildFromNode(CharTrieNode node, char key)
    {
        for (int i = 0; i < node.Children.Length; i++)
        {
            if (_comparer.Equals(key, node.Children[i].Key))
            {
                node.RemoveChildAt(i);

                break;
            }
        }
    }

    private CharTrieNode? GetChildNode(CharTrieNode node, char key)
    {
        for (var i = 0; i < node.Children.Length; i++)
        {
            var n = node.Children[i];

            if (_comparer.Equals(key, n.Key))
            {
                return n;
            }
        }

        return null;
    }
}