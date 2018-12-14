using System;

namespace MonoGame.Extended.BitmapFonts
{
    public interface ICharIterator : IDisposable
    {
        int Length { get; }

        int GetCharacter32(ref int index);
        char GetCharacter16(int index);
    }
}
