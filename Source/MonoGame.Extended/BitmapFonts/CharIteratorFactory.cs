using System;
using System.Collections.Concurrent;
using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    public static class CharIteratorPool
    {
        public const int LargeBuilderThreshold = 360;
        public const int CharBufferSize = 1024 * 8;

        private static ConcurrentStack<StringBuilder> _smallBuilders;
        private static ConcurrentStack<StringBuilder> _largeBuilders;

        [ThreadStatic]
        private static WeakReference<char[]> _charBuffer;

        private static ConcurrentStack<StringCharIterator> _stringIterators;
        private static ConcurrentStack<StringBuilderCharIterator> _builderIterators;

        static CharIteratorPool()
        {
            _smallBuilders = new ConcurrentStack<StringBuilder>();
            _largeBuilders = new ConcurrentStack<StringBuilder>();

            _stringIterators = new ConcurrentStack<StringCharIterator>();
            _builderIterators = new ConcurrentStack<StringBuilderCharIterator>();
        }

        public static ICharIterator RentIterator(string value, int offset, int count)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            if (_stringIterators.TryPop(out StringCharIterator result))
            {
                result.Set(value, offset, count);
                return result;
            }
            else
                return new StringCharIterator(value, offset, count);
        }

        public static StringBuilder RentSmallBuilder()
        {
            if (_smallBuilders.TryPop(out StringBuilder result))
            {
                result.Clear();
                return result;
            }
            return new StringBuilder();
        }

        public static StringBuilder RentLargeBuilder()
        {
            if (_largeBuilders.TryPop(out StringBuilder result))
            {
                result.Clear();
                return result;
            }
            return new StringBuilder(LargeBuilderThreshold);
        }

        public static ICharIterator RentIterator(StringBuilder value, int offset, int count)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            StringBuilder immutableBuilder = count < LargeBuilderThreshold ?
                RentSmallBuilder() : RentLargeBuilder();

            char[] buffer = GetCharBuffer();
            value.CopyTo(offset, immutableBuilder, buffer, count);

            if (_builderIterators.TryPop(out StringBuilderCharIterator result))
            {
                result.Set(immutableBuilder, offset, count);
                return result;
            }
            else
                return new StringBuilderCharIterator(immutableBuilder, offset, count);
        }

        public static void ReturnBuilder(StringBuilder builder)
        {
            if (builder.Capacity < LargeBuilderThreshold)
            {
                if (_largeBuilders.Count < 128)
                    _smallBuilders.Push(builder);
            }
            else
            {
                if (_largeBuilders.Count < 64)
                    _largeBuilders.Push(builder);
            }
        }

        public static void ReturnIterator(ICharIterator iterator)
        {
            if (iterator is StringCharIterator stringIterator)
            {
                stringIterator.Set(null, 0, 0);
                _stringIterators.Push(stringIterator);
            }
            else if (iterator is StringBuilderCharIterator builderIterator)
            {
                ReturnBuilder(builderIterator._builder);
                builderIterator._builder = null;

                _builderIterators.Push(builderIterator);
            }
            else
                throw new ArgumentException("The iterator was not rented from this pool.");
        }

        private static char[] GetCharBuffer()
        {
            char[] buffer;
            if (_charBuffer == null)
            {
                buffer = new char[CharBufferSize];
                _charBuffer = new WeakReference<char[]>(buffer);
            }
            else if (!_charBuffer.TryGetTarget(out buffer))
            {
                buffer = new char[CharBufferSize];
                _charBuffer.SetTarget(buffer);
            }
            return buffer;
        }
    }
}
