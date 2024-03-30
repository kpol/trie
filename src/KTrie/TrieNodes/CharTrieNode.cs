using System;

namespace KTrie.TrieNodes;

internal class CharTrieNode(char key)
{
    public char Key { get; } = key;

    public virtual bool IsTerminal => false;

    public CharTrieNode[] Children { get; set; } = [];

    public void AddChild(CharTrieNode node)
    {
        var children = new CharTrieNode[Children.Length + 1];
        Array.Copy(Children, children, Children.Length);
        children[^1] = node;
        Children = children;
    }

    public void RemoveChildAt(int index)
    {
        var children = new CharTrieNode[Children.Length - 1];
        Children[index] = Children[^1];
        Array.Copy(Children, children, children.Length);
        Children = children;
    }

    public void CopyChildren(CharTrieNode[] toCopy)
    {
        Children = new CharTrieNode[toCopy.Length];
        Array.Copy(toCopy, Children, Children.Length);
    }

    public override string ToString() => $"Key: {Key}";
}