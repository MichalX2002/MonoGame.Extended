using System;
using System.Text;

namespace MonoGame.Extended.BitmapFonts
{
    public partial class BitmapFont
    {
        public interface ITextIterator
        {
            int Offset { get; }
            int Count { get; }
            int ValueLength { get; }

            int GetCharacter(ref int index);
        }

        private readonly struct StringBuilderTextIterator : ITextIterator
        {
            private readonly StringBuilder _text;

            public int Offset { get; }
            public int Count { get; }
            public int ValueLength => _text.Length;

            public StringBuilderTextIterator(StringBuilder text, int offset, int length)
            {
                _text = text ?? throw new ArgumentNullException(nameof(text));
                Offset = offset;
                Count = length;
            }

            public int GetCharacter(ref int index)
            {
                return char.IsHighSurrogate(_text[index]) && ++index < _text.Length
                    ? char.ConvertToUtf32(_text[index - 1], _text[index])
                    : _text[index];
            }
        }

        private readonly struct StringTextIterator : ITextIterator
        {
            private readonly string _text;

            public int Offset { get; }
            public int Count { get; }
            public int ValueLength => _text.Length;

            public StringTextIterator(string text, int offset, int length)
            {
                _text = text ?? throw new ArgumentNullException(nameof(text));
                Offset = offset;
                Count = length;
            }

            public int GetCharacter(ref int index)
            {
                return char.IsHighSurrogate(_text[index]) && ++index < _text.Length
                    ? char.ConvertToUtf32(_text[index - 1], _text[index])
                    : _text[index];
            }
        }
    }
}
