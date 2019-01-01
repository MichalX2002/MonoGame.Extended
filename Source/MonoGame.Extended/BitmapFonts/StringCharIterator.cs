using System;
using System.Diagnostics;

namespace MonoGame.Extended.BitmapFonts
{
    internal class StringCharIterator : ICharIterator
    {
        private string _cachedString;

        private string _value;
        internal bool _isInUse;
        private int _offset;

        public int Length { get; private set; }

        public StringCharIterator(string value, int offset, int length)
        {
            Set(value, offset, length);
        }

        public void Set(string value, int offset, int length)
        {
            _value = value;
            _offset = offset;
            Length = length;
            _isInUse = _value != null;

            _cachedString = null;
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
            char firstChar = _value[index + _offset];
            return char.IsHighSurrogate(firstChar) && (++index + _offset) < Length
                ? char.ConvertToUtf32(firstChar, _value[index + _offset])
                : firstChar;
        }

        public char GetCharacter16(int index)
        {
            CheckIfInUse();
            return _value[index + _offset];
        }

        public string GetString()
        {
            CheckIfInUse();

            if (_cachedString == null)
                _cachedString = _value.Substring(_offset, Length);
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
