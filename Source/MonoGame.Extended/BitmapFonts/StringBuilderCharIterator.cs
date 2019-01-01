using System;
using System.Diagnostics;
using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    internal class StringBuilderCharIterator : ICharIterator
    {
        private string _cachedString;

        internal StringBuilder _builder;
        internal bool _isInUse;

        public int Length { get; private set; }

        public StringBuilderCharIterator(StringBuilder builder, int length)
        {
            Set(builder, length);
        }

        internal void Set(StringBuilder builder, int length)
        {
            _builder = builder;
            Length = length;
            _isInUse = builder != null;

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
            char firstChar = _builder[index];
            return char.IsHighSurrogate(firstChar) && ++index < Length
                ? char.ConvertToUtf32(firstChar, _builder[index])
                : firstChar;
        }

        public char GetCharacter16(int index)
        {
            CheckIfInUse();
            return _builder[index];
        }

        public string GetString()
        {
            CheckIfInUse();

            if (_cachedString == null)
                _cachedString = _builder.ToString().Substring(0, Length);
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
