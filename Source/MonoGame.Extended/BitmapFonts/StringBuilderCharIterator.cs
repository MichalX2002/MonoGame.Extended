using System;
using System.Diagnostics;
using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    internal class StringBuilderCharIterator : ICharIterator
    {
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

        public void Dispose()
        {
            if (_isInUse)
                CharIteratorPool.Return(this);
        }
    }
}
