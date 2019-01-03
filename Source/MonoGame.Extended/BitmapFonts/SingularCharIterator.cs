using System;
using System.Diagnostics;

namespace MonoGame.Extended.BitmapFonts
{
    internal class SingularCharIterator : ICharIterator
    {
        private string _cachedString;
        private char _firstChar;
        private char _lowSurrogate;

        internal bool _isInUse;

        public int Length { get; private set; }

        public SingularCharIterator(char value)
        {
            Set(value);
        }

        public SingularCharIterator(char highSurrogate, char lowSurrogate)
        {
            Set(highSurrogate, lowSurrogate);
        }

        private void Set()
        {
            _isInUse = true;
            _cachedString = null;
        }

        internal void Set(char value)
        {
            _firstChar = value;
            Length = 1;
            Set();
        }

        internal void Set(char highSurrogate, char lowSurrogate)
        {
            _firstChar = highSurrogate;
            _lowSurrogate = lowSurrogate;
            Length = 2;
            Set();
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

            if (Length == 2)
            {
                index++;
                return char.ConvertToUtf32(_firstChar, _lowSurrogate);
            }
            return _firstChar;
        }

        public char GetCharacter16(int index)
        {
            CheckIfInUse();
            if (index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _firstChar;
        }

        public string GetString()
        {
            CheckIfInUse();
            if (_cachedString == null)
            {
                if (Length == 2)
                {
                    int utf32 = char.ConvertToUtf32(_firstChar, _lowSurrogate);
                    _cachedString = char.ConvertFromUtf32(utf32);
                }
                else
                    _cachedString = _firstChar.ToString();
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
