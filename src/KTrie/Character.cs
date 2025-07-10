namespace KTrie;

public readonly record struct Character(char? Char)
{
    public static readonly Character Any = new();

    public static implicit operator Character(char c) => new(c);
}