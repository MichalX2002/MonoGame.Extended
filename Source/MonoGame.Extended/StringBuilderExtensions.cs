using System;
using System.Text;

namespace MonoGame.Extended
{
    public static class StringBuilderExtensions
    {
        public static void CopyTo(
            this StringBuilder src, int srcIndex,
            StringBuilder dst, char[] buffer, int count)
        {
            if (src.Length < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            int left = count;
            dst.EnsureCapacity(left);

            while (left > 0)
            {
                int c = Math.Min(buffer.Length, left);
                src.CopyTo(src.Length - left, buffer, 0, c);
                dst.Append(buffer, 0, c);
                left -= c;
            }
        }
    }
}
