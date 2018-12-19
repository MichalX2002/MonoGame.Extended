using MonoGame.Extended.Collections;
using System;
using System.Diagnostics;
using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    public static class CharIteratorPool
    {
        public const int CharBufferSize = 1024 * 8;
        public const int DefaultPoolCapacity = 512;
        public const int MaxPoolCapacity = 1024 * 16;

        [ThreadStatic]
        private static char[] _charBuffer;

        private static Bag<StringCharIterator> _stringIterators;
        private static Bag<StringBuilderCharIterator> _builderIterators;

        private static int _poolCapacity = DefaultPoolCapacity;
        public static int PoolCapacity
        {
            get => _poolCapacity;
            set
            {
                if (value < 0 || value > MaxPoolCapacity)
                    throw new ArgumentException(nameof(value));
                _poolCapacity = value;
            }
        }

        static CharIteratorPool()
        {
            _stringIterators = new Bag<StringCharIterator>();
            _builderIterators = new Bag<StringBuilderCharIterator>();
        }

        public static ICharIterator Rent(string value, int offset, int count)
        {
            if (count == 0)
                return EmptyCharIterator.Instance;

            if (value == null)
                throw new ArgumentNullException(nameof(value));
            CheckArguments(value.Length, offset, count);

            lock (_stringIterators)
            {
                if (_stringIterators.TryTake(out var result))
                {
                    result.Set(value, offset, count);
                    return result;
                }
            }
            return new StringCharIterator(value, offset, count);
        }

        public static ICharIterator Rent(string value)
        {
            return Rent(value, 0, value.Length);
        }

        public static ICharIterator Rent(StringBuilder value, int offset, int count)
        {
            if (count == 0)
                return EmptyCharIterator.Instance;

            if (value == null)
                throw new ArgumentNullException(nameof(value));
            CheckArguments(value.Length, offset, count);
            
            var immutableBuilder = StringBuilderPool.Rent(count);
            char[] buffer = GetCharBuffer();
            value.CopyTo(offset, immutableBuilder, buffer, count);
            
            lock (_builderIterators)
            {
                if (_builderIterators.TryTake(out var result))
                {
                    result.Set(immutableBuilder, count);
                    return result;
                }
            }
            return new StringBuilderCharIterator(immutableBuilder, count);
        }

        public static ICharIterator Rent(StringBuilder value)
        {
            return Rent(value, 0, value.Length);
        }

        [DebuggerHidden]
        private static void CheckArguments(int length, int offset, int count)
        {
            if (count > length)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (offset + count > length)
                throw new ArgumentOutOfRangeException(nameof(offset));
        }

        /// <summary>
        /// Return a <see cref="ICharIterator"/> previously rented from this pool.
        /// </summary>
        /// <param name="iterator">The iterator to return to the pool.</param>
        /// <exception cref="ArgumentException">The iterator was not rented from this pool.</exception>
        /// <remarks>This method does not throw if <see cref="EmptyCharIterator.Instance"/> is passed as the argument.</remarks>
        public static void Return(ICharIterator iterator)
        {
            if (iterator == null)
                return;

            if (iterator is StringCharIterator stringIterator)
            {
                lock (_stringIterators)
                {
                    if (stringIterator._isInUse && _stringIterators.Count < _poolCapacity)
                        _stringIterators.Add(stringIterator);

                    stringIterator.Set(null, 0, 0);
                }
            }
            else if (iterator is StringBuilderCharIterator builderIterator)
            {
                lock (_builderIterators)
                {
                    if (builderIterator._isInUse && _builderIterators.Count < _poolCapacity)
                        _builderIterators.Add(builderIterator);

                    StringBuilderPool.Return(builderIterator._builder);
                    builderIterator.Set(null, 0);
                }
            }
            else if(!(iterator is EmptyCharIterator))
                throw new ArgumentException("The iterator was not rented from this pool.");
        }

        private static char[] GetCharBuffer()
        {
            // _charBuffer is thread-static, no locks needed here
            if (_charBuffer == null)
                _charBuffer = new char[CharBufferSize];
            return _charBuffer;
        }
    }
}
