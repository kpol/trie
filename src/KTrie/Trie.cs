using KTrie.TrieNodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KTrie;

public sealed class Trie(IEqualityComparer<char>? comparer = null) 
    : ICollection<string>, IReadOnlyCollection<string>
{
    private readonly IEqualityComparer<char> _comparer = comparer ?? EqualityComparer<char>.Default;

    private readonly CharTrieNode _root = new(char.MinValue);

    public int Count { get; private set; }

    bool ICollection<string>.IsReadOnly => false;

    public void Add(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        TryAdd(word);
    }

    public bool TryAdd(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        var node = AddNodesFromUpToBottom(word);

        if (node.existingTerminalNode is not null && node.existingTerminalNode.IsTerminal) return false; // already exists

        var newTerminalNode = new TerminalCharTrieNode(word[^1]) { Word = word };

        AddTerminalNode(node.parent, node.existingTerminalNode, newTerminalNode, word);

        return true;
    }

    public void Clear() => _root.Children.Clear();

    public bool Contains(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        var node = GetNode(word);

        return node is not null && node.IsTerminal;
    }

    public bool Remove(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        var nodesUpToBottom = GetNodesForRemoval(word);

        if (nodesUpToBottom.Count == 0) return false;

        RemoveNode(nodesUpToBottom);

        return true;
    }

    public IEnumerable<string> GetByPrefix(string prefix)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);

        return _();

        IEnumerable<string> _()
        {
            foreach (var n in GetTerminalNodesByPrefix(prefix))
            {
                yield return n.Word;
            }
        }
    }

    internal (CharTrieNode? existingTerminalNode, CharTrieNode parent) AddNodesFromUpToBottom(string word)
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

    internal void AddTerminalNode(CharTrieNode parent, CharTrieNode? existingNode, CharTrieNode newTerminalNode, string word)
    {
        if (existingNode is not null)
        {
            newTerminalNode.Children.AddRange(existingNode.Children);
            RemoveChildFromNode(parent, word[^1]);
        }

        AddToNode(parent, newTerminalNode);
        Count++;
    }

    internal IEnumerable<TerminalCharTrieNode> GetTerminalNodesByPrefix(string prefix)
    {
        var node = GetNode(prefix);

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

    private static IEnumerable<TerminalCharTrieNode> GetDescendantTerminalNodes(CharTrieNode node)
    {
        Stack<CharTrieNode> stack = new(node.Children);

        while (stack.Count > 0)
        {
            var n = stack.Pop();

            if (n.IsTerminal)
            {
                yield return (TerminalCharTrieNode)n;
            }

            foreach (var childNode in n.Children)
            {
                stack.Push(childNode);
            }
        }
    }

    internal CharTrieNode? GetNode(string prefix)
    {
        var current = _root;

        foreach (var c in prefix)
        {
            current = GetChildNode(current, c);

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    private Stack<CharTrieNode> GetNodesForRemoval(string prefix)
    {
        var current = _root;

        Stack<CharTrieNode> nodesUpToBottom = [];
        nodesUpToBottom.Push(_root);

        foreach (var c in prefix)
        {
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

        if (node.Children.Count == 0)
        {
            while (node.Children.Count == 0 && nodesUpToBottom.Count > 0)
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
            n.Children.AddRange(node.Children);

            var parent = nodesUpToBottom.Count == 0 ? _root : nodesUpToBottom.Pop();

            RemoveChildFromNode(parent, node.Key);
            AddToNode(parent, n);
        }
    }

    private void AddToNode(CharTrieNode node, CharTrieNode nodeToAdd)
    {
        foreach (var n in node.Children)
        {
            if (_comparer.Equals(nodeToAdd.Key, n.Key))
            {
                return;
            }
        }

        node.Children.Add(nodeToAdd);
    }

    private void RemoveChildFromNode(CharTrieNode node, char key)
    {
        int index = -1;

        for (int i = 0; i < node.Children.Count; i++)
        {
            if (_comparer.Equals(key, node.Children[i].Key))
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            node.Children.RemoveAt(index);
        }
    }

    private CharTrieNode? GetChildNode(CharTrieNode node, char key)
    {
        foreach (var n in node.Children)
        {
            if (_comparer.Equals(key, n.Key))
            {
                return n;
            }
        }

        return null;
    }
}