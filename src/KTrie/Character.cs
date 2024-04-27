namespace KTrie;

public readonly record struct Character(char c) 
{
    public static Character Any => AnyCharacter.Instance;

    public char Char => c;

    public static implicit operator Character(char c) => new(c);

    private static class AnyCharacter
    {
        public static readonly Character Instance = new(char.MinValue);
    }
}