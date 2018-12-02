
using System;

namespace MonoGame.Extended.BitmapFonts
{
    internal class StringCharIterator : ICharIterator
    {
        private string _value;
        internal bool _isInUse;

        public int Offset { get; private set; }
        public int Count { get; private set; }
        public int TotalCount => _value.Length;

        public StringCharIterator(string value, int offset, int count)
        {
            Set(value, offset, count);
        }

        public void Set(string value, int offset, int count)
        {
            _value = value;
            Offset = offset;
            Count = count;
            _isInUse = _value != null;
        }

        public int GetCharacter(ref int index)
        {
            if (_isInUse == false)
                throw new InvalidOperationException("This iterator is no longer valid.");

            return char.IsHighSurrogate(_value[index]) && ++index < TotalCount
                ? char.ConvertToUtf32(_value[index - 1], _value[index])
                : _value[index];
        }

        public void Dispose()
        {
            if (_isInUse)
                CharIteratorPool.Return(this);
        }
    }
}
