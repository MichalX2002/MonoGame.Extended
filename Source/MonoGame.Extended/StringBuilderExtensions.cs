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
            if (count > src.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            if (srcIndex + count > src.Length)
                throw new ArgumentOutOfRangeException(nameof(srcIndex));

            int left = count;
            dst.EnsureCapacity(left);

            while (left > 0)
            {
                int c = Math.Min(buffer.Length, left);
                src.CopyTo(srcIndex + count - left, buffer, 0, c);
                dst.Append(buffer, 0, c);
                left -= c;
            }
        }
    }
}
