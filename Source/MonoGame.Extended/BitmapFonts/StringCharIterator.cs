
namespace MonoGame.Extended.BitmapFonts
{
    internal class StringCharIterator : ICharIterator
    {
        private string _value;

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
        }

        public int GetCharacter(ref int index)
        {
            return char.IsHighSurrogate(_value[index]) && ++index < TotalCount
                ? char.ConvertToUtf32(_value[index - 1], _value[index])
                : _value[index];
        }
    }
}
