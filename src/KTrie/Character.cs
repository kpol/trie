namespace KTrie;

public readonly record struct Character(char? Char)
{
    public static Character Any { get; } = new();

    public static implicit operator Character(char c) => new(c);
}