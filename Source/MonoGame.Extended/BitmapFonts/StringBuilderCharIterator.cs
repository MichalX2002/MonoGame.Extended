using System;
using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    internal class StringBuilderCharIterator : ICharIterator
    {
        internal StringBuilder _builder;
        internal bool _isInUse;

        public int Offset { get; private set; }
        public int Count { get; private set; }
        public int TotalCount => _builder.Length;
        
        public StringBuilderCharIterator(StringBuilder value, int offset, int count)
        {
            Set(value, offset, count);
        }

        internal void Set(StringBuilder value, int offset, int count)
        {
            _builder = value;
            Offset = offset;
            Count = count;
            _isInUse = value != null;
        }

        public int GetCharacter(ref int index)
        {
            if (_isInUse == false)
                throw new InvalidOperationException("This iterator is no longer valid.");

            return char.IsHighSurrogate(_builder[index]) && ++index < TotalCount
                ? char.ConvertToUtf32(_builder[index - 1], _builder[index])
                : _builder[index];
        }

        public void Dispose()
        {
            if (_isInUse)
                CharIteratorPool.Return(this);
        }
    }
}
