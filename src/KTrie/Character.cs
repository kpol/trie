namespace KTrie;

public class Character(char c)
{
    public static Character Any => AnyCharacter.Instance;

    public char Char => c;

    public static implicit operator Character(char c) => new(c);

    private class AnyCharacter : Character
    {
        public static readonly AnyCharacter Instance = new();

        private AnyCharacter() : base(char.MinValue)
        {

        }
    }
}