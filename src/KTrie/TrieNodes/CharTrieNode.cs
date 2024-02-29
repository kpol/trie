using System.Collections.Generic;

namespace KTrie.TrieNodes;

internal class CharTrieNode(char key)
{
    public char Key { get; } = key;

    public virtual bool IsTerminal => false;

    public List<CharTrieNode> Children { get; } = [];

    public override string ToString() => $"Key: {Key}";
}