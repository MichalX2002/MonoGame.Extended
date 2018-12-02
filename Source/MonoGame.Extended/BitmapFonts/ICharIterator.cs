using System;

namespace MonoGame.Extended.BitmapFonts
{
    public interface ICharIterator : IDisposable
    {
        int Offset { get; }
        int Count { get; }

        int GetCharacter(ref int index);
    }
}
