using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    public static class CharIteratorExtensions
    {
        public static ICharIterator ToIterator(this StringBuilder value, int offset, int count)
        {
            return CharIteratorPool.Rent(value, offset, count);
        }

        public static ICharIterator ToIterator(this StringBuilder value)
        {
            return ToIterator(value, 0, value.Length);
        }

        public static ICharIterator ToIterator(this string value, int offset, int count)
        {
            return CharIteratorPool.Rent(value, offset, count);
        }

        public static ICharIterator ToIterator(this string value)
        {
            return ToIterator(value, 0, value.Length);
        }

        public static ICharIterator ToIterator(this char value)
        {
            return CharIteratorPool.Rent(value);
        }

        public static ICharIterator ToIterator(this char highSurrogate, char lowSurrogate)
        {
            return CharIteratorPool.Rent(highSurrogate, lowSurrogate);
        }

        public static ICharIterator ToRepeatingIterator(this char value, int length)
        {
            return CharIteratorPool.RentRepeater(value, length);
        }

        public static ICharIterator ToRepeatingIterator(this char highSurrogate, char lowSurrogate, int length)
        {
            return CharIteratorPool.RentRepeater(highSurrogate, lowSurrogate, length);
        }
    }
}
