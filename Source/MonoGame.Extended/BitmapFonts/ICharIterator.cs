
namespace MonoGame.Extended.BitmapFonts
{
    public interface ICharIterator
    {
        int Offset { get; }
        int Count { get; }

        int GetCharacter(ref int index);
    }
}
