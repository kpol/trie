namespace KTrie.TrieNodes;

internal class TerminalCharTrieNode(char key) : CharTrieNode(key)
{
    public override bool IsTerminal => true;

    public string Word { get; init; } = null!;

    public override string ToString() => $"Key: {Key}, Word: {Word}";
}