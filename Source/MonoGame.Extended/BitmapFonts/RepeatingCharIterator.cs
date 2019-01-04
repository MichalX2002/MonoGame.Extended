using System;
using System.Diagnostics;

namespace MonoGame.Extended.BitmapFonts
{
    internal class RepeatingCharIterator : ICharIterator
    {
        private string _cachedString;
        private char _firstChar;
        private char _lowSurrogateChar;
        private bool _isUtf32;

        internal bool _isInUse;

        public int Length { get; private set; }

        public RepeatingCharIterator(char value, int length)
        {
            Set(value, length);
        }

        public RepeatingCharIterator(char highSurrogate, char lowSurrogate, int length)
        {
            Set(highSurrogate, lowSurrogate, length);
        }

        private void Set(int length, bool utf32)
        {
            Length = length;
            _isUtf32 = utf32;
            _isInUse = true;
            _cachedString = null;
        }

        internal void Set(char value, int length)
        {
            _firstChar = value;
            Set(length, utf32: false);
        }

        internal void Set(char highSurrogate, char lowSurrogate, int length)
        {
            _firstChar = highSurrogate;
            _lowSurrogateChar = lowSurrogate;
            Set(length, utf32: true);
        }

        [DebuggerHidden]
        private void CheckIfInUse()
        {
            if (!_isInUse)
                throw new InvalidOperationException("This iterator is no longer valid.");
        }

        public int GetCharacter32(ref int index)
        {
            CheckIfInUse();
            if (index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_isUtf32)
            {
                if (index % 2 == 1)
                    return _lowSurrogateChar;

                index++;
                return char.ConvertToUtf32(_firstChar, _lowSurrogateChar);
            }
            return _firstChar;
        }

        public char GetCharacter16(int index)
        {
            CheckIfInUse();
            if (index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (_isUtf32 && index % 2 == 1)
                return _lowSurrogateChar;
            return _firstChar;
        }

        public string GetString()
        {
            CheckIfInUse();
            if (_cachedString == null)
            {
                var builder = StringBuilderPool.Rent(Length);
                for (int i = 0; i < Length; i++)
                    builder.Append(GetCharacter16(i));

                _cachedString = builder.ToString();
                StringBuilderPool.Return(builder);
            }
            return _cachedString;
        }

        public override string ToString()
        {
            if (!_isInUse)
                return string.Empty;
            return GetString();
        }

        public void Dispose()
        {
            if (_isInUse)
                CharIteratorPool.Return(this);
        }
    }
}
