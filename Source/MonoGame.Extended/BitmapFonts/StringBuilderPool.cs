using MonoGame.Extended.Collections;
using System;
using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    public static class StringBuilderPool
    {
        public const int DefaultSmallPoolCapacity = 256;
        public const int MaxSmallPoolCapacity = 1024 * 16;

        public const int DefaultLargePoolCapacity = 64;
        public const int MaxLargePoolCapacity = 1024 * 4;
        public const int LargeBuilderThreshold = 360;
        public const int MaxLargeBuilderSize = 1024 * 16;
        
        private static int _smallPoolCapacity = DefaultSmallPoolCapacity;
        public static int SmallPoolCapacity
        {
            get => _smallPoolCapacity;
            set
            {
                if (value < 0 || value > MaxSmallPoolCapacity)
                    throw new ArgumentException(nameof(value));
                _smallPoolCapacity = value;
            }
        }

        private static int _largePoolCapacity = DefaultLargePoolCapacity;
        public static int LargePoolCapacity
        {
            get => _largePoolCapacity;
            set
            {
                if (value < 0 || value > MaxLargePoolCapacity)
                    throw new ArgumentException(nameof(value));
                _largePoolCapacity = value;
            }
        }

        private static Bag<StringBuilder> _smallBuilders;
        private static Bag<StringBuilder> _largeBuilders;

        static StringBuilderPool()
        {
            _smallBuilders = new Bag<StringBuilder>();
            _largeBuilders = new Bag<StringBuilder>();
        }

        public static StringBuilder Rent(int expectedLength)
        {
            return expectedLength >= LargeBuilderThreshold ? RentLarge() : RentSmall();
        }

        public static StringBuilder RentSmall()
        {
            lock (_smallBuilders)
            {
                if (_smallBuilders.TryTake(out var result))
                {
                    result.Clear();
                    return result;
                }
            }
            return new StringBuilder();
        }

        public static StringBuilder RentLarge()
        {
            lock (_largeBuilders)
            {
                if (_largeBuilders.TryTake(out var result))
                {
                    result.Clear();
                    return result;
                }
            }
            return new StringBuilder(LargeBuilderThreshold);
        }

        public static void Return(StringBuilder builder)
        {
            if (builder.Capacity >= LargeBuilderThreshold)
            {
                if (builder.Capacity > MaxLargeBuilderSize)
                    return;

                lock (_largeBuilders)
                {
                    if (_largeBuilders.Count < _largePoolCapacity)
                        _largeBuilders.Add(builder);
                }
            }
            else
            {
                lock (_smallBuilders)
                {
                    if (_smallBuilders.Count < _smallPoolCapacity)
                        _smallBuilders.Add(builder);
                }
            }
        }
    }
}
