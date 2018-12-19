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
    }
}
