using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KTrie;

public sealed class StringTrieSet2 : ICollection<string>
{
    private readonly IEqualityComparer<char> _comparer;

    private readonly TrieNode _root;

    public StringTrieSet2(IEqualityComparer<char> comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<char>.Default;
        _root = new TrieNode(char.MinValue, _comparer);
    }

    public int Count { get; private set; }

    public bool IsReadOnly => false;

    public void Add(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        if (string.IsNullOrEmpty(word))
        {
            return;
        }

        TrieNode current = _root;

        foreach (var c in word)
        {
            if (current.TryGetChildNode(c, out var value))
            {
                current = value;
            }
            else
            {
                TrieNode node = new(c, _comparer)
                {
                    Parent = current
                };
                current.Add(node);
                current = node;
            }
        }

        current.Word = word;
        Count++;
    }

    public void AddRange(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        foreach (var word in words)
        {
            Add(word);
        }
    }

    public void Clear() => _root.Clear();

    public bool Contains(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        var node = GetNode(word);

        return node is not null && node.IsTerminal;
    }

    public void CopyTo(string[] array, int arrayIndex) => Array.Copy(GetDescendants(_root).Select(n => n.Word).ToArray(), 0, array, arrayIndex, Count);

    public bool Remove(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        var node = GetNode(word);

        if (node is null || !node.IsTerminal) return false;

        RemoveNode(node);

        return true;
    }

    public IEnumerable<string> GetByPrefix(string prefix)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);

        var node = GetNode(prefix);

        if (node is null)
        {
            yield break;
        }

        foreach (var n in GetDescendants(node))
        {
            yield return n.Word;
        }
    }

    public IEnumerator<string> GetEnumerator() => GetDescendants(_root).Select(n => n.Word).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static IEnumerable<TrieNode> GetDescendants(TrieNode node)
    {
        Stack<TrieNode> st = new();
        st.Push(node);

        while (st.Count > 0)
        {
            var n = st.Pop();

            if (n.IsTerminal)
            {
                yield return n;
            }

            foreach (var nodeChild in n)
            {
                st.Push(nodeChild);
            }
        }
    }

    private TrieNode GetNode(string prefix)
    {
        TrieNode current = _root;

        foreach (var t in prefix)
        {
            if (current.TryGetChildNode(t, out var value))
            {
                current = value;
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    private void RemoveNode(TrieNode node)
    {
        Count--;

        node.Word = null;

        while (true)
        {
            if (node.ChildrenCount == 0 && node.Parent is not null)
            {
                node.Parent.RemoveChild(node.Key);

                if (!node.Parent.IsTerminal)
                {
                    node = node.Parent;
                    continue;
                }
            }

            break;
        }
    }

    private sealed class TrieNode(char key, IEqualityComparer<char> comparer) : IEnumerable<TrieNode>
    {
        private readonly List<TrieNode> _children = [];

        public char Key { get; } = key;

        public TrieNode Parent { get; init; }

        public bool IsTerminal => Word is not null;

        public string Word { get; set; }

        public int ChildrenCount => _children.Count;

        public void Add(TrieNode node) => _children.Add(node);

        public void RemoveChild(char key)
        {
            int index = -1;

            for (int i = 0; i < _children.Count; i++)
            {
                if (comparer.Equals(key, _children[i].Key))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                _children.RemoveAt(index);
            }
        }

        public bool TryGetChildNode(char key, out TrieNode node)
        {
            foreach (var n in _children)
            {
                if (comparer.Equals(key, n.Key))
                {
                    node = n;

                    return true;
                }
            }

            node = default;

            return false;
        }

        public void Clear() => _children.Clear();

        public IEnumerator<TrieNode> GetEnumerator() => _children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => IsTerminal ? $"{Key}:{Word}" : Key.ToString();
    }
}