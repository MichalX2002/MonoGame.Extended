using System;

namespace MonoGame.Extended.BitmapFonts
{
    /// <summary>
    /// Specialized read-only list for Unicode characters.
    /// </summary>
    public interface ICharIterator : IDisposable
    {
        /// <summary>
        /// The amount of characters this <see cref="ICharIterator"/> exposes.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Returns the UTF-32 character at the given index.
        /// </summary>
        /// <param name="index">The character index.</param>
        /// <returns>The UTF-32 character.</returns>
        int GetCharacter32(ref int index);

        /// <summary>
        /// Returns the UTF-16 character at the given index.
        /// </summary>
        /// <param name="index">The character index.</param>
        /// <returns>The UTF-16 character.</returns>
        char GetCharacter16(int index);

        /// <summary>
        /// Returns the <see langword="string"/> representation of this <see cref="ICharIterator"/>.
        /// </summary>
        /// <returns>The <see langword="string"/> representation of this <see cref="ICharIterator"/>.</returns>
        string GetString();
    }
}
