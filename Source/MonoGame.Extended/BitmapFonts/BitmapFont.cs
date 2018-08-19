using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFont
    {
        public struct Glyph
        {
            public int Character;
            public Vector2 Position;
            public BitmapFontRegion FontRegion;
        }

        private readonly IReadOnlyDictionary<int, BitmapFontRegion> _characterMap;

        public BitmapFont(string name, IEnumerable<BitmapFontRegion> regions, int lineHeight) :
            this(name, lineHeight)
        {
            int count = regions is ICollection collection ? collection.Count : 0;
            var temp = new Dictionary<int, BitmapFontRegion>(count);
            foreach (var region in regions)
                temp.Add(region.Character, region);
            _characterMap = temp;
        }

        public BitmapFont(string name, IReadOnlyDictionary<int, BitmapFontRegion> regions, int lineHeight) :
            this(name, lineHeight)
        {
            _characterMap = regions;
        }

        private BitmapFont(string name, int lineHeight)
        {
            Name = name;
            LineHeight = lineHeight;
        }

        public string Name { get; }
        public int LineHeight { get; }
        public int LetterSpacing { get; set; } = 0;
        public static bool UseKernings { get; set; } = true;

        public BitmapFontRegion GetCharacterRegion(int character)
        {
            return _characterMap.TryGetValue(character, out BitmapFontRegion region) ? region : null;
        }

        public bool GetCharacterRegion(int character, out BitmapFontRegion region)
        {
            return _characterMap.TryGetValue(character, out region);
        }

        public SizeF MeasureString(string text, int offset, int length)
        {
            if (string.IsNullOrEmpty(text))
                return SizeF.Empty;

            return GetStringRectangle(text, Point2.Zero).Size;
        }

        public SizeF MeasureString(StringBuilder text, int offset, int length)
        {
            if (text == null || text.Length == 0)
                return SizeF.Empty;

            return GetStringRectangle(text, Point2.Zero).Size;
        }

        public SizeF MeasureString(string text)
        {
            return MeasureString(text, 0, text.Length);
        }

        public SizeF MeasureString(StringBuilder text)
        {
            return MeasureString(text, 0, text.Length);
        }

        public RectangleF GetStringRectangle(
            string text, int offset, int length, Point2 position)
        {
            var enumerable = GetGlyphs(text, offset, length, position);
            return GetStringRectangle(ref enumerable.Glyphs, position);
        }

        public RectangleF GetStringRectangle(
            StringBuilder text, int offset, int length, Point2 position)
        {
            var enumerable = GetGlyphs(text, offset, length, position);
            return GetStringRectangle(ref enumerable.Glyphs, position);
        }

        public RectangleF GetStringRectangle(string text, Point2 position)
        {
            return GetStringRectangle(text, 0, text.Length, position);
        }

        public RectangleF GetStringRectangle(StringBuilder text, Point2 position)
        {
            return GetStringRectangle(text, 0, text.Length, position);
        }

        private RectangleF GetStringRectangle(ref GlyphEnumerator iterator, Point2 position)
        {
            var rectangle = new RectangleF(position.X, position.Y, 0, LineHeight);
            
            while (iterator.MoveNext())
            {
                ref Glyph glyph = ref iterator.CurrentGlyph;

                if (glyph.FontRegion != null)
                {
                    var right = glyph.Position.X + glyph.FontRegion.Width;

                    if (right > rectangle.Right)
                        rectangle.Width = right - rectangle.Left;
                }

                if (glyph.Character == '\n')
                    rectangle.Height += LineHeight;
            }

            return rectangle;
        }

        public StringGlyphEnumerable GetGlyphs(
            string text, int offset, int length, Point2 position)
        {
            return new StringGlyphEnumerable(this, text, offset, length, position);
        }

        public StringBuilderGlyphEnumerable GetGlyphs(
            StringBuilder text, int offset, int length, Point2 position)
        {
            return new StringBuilderGlyphEnumerable(this, text, offset, length, position);
        }

        public StringGlyphEnumerable GetGlyphs(string text, Point2 position)
        {
            return GetGlyphs(text, 0, text.Length, position);
        }

        public StringBuilderGlyphEnumerable GetGlyphs(StringBuilder text, Point2 position)
        {
            return GetGlyphs(text, 0, text.Length, position);
        }

        public StringGlyphEnumerable GetGlyphs(string text)
        {
            return GetGlyphs(text, Point2.Zero);
        }

        public StringBuilderGlyphEnumerable GetGlyphs(StringBuilder text)
        {
            return GetGlyphs(text, Point2.Zero);
        }

        public override string ToString()
        {
            return Name;
        }
        
        public interface ITextIterator
        {
            int Offset { get; }
            int Length { get; }
            
            int GetCharacter(ref int index);
        }

        private struct StringBuilderTextIterator : ITextIterator
        {
            private StringBuilder _text;

            public int Offset { get; }
            public int Length { get; }

            public StringBuilderTextIterator(StringBuilder text, int offset, int length)
            {
                _text = text ?? throw new ArgumentNullException(nameof(text));
                Offset = offset;
                Length = length;
            }

            public int GetCharacter(ref int index)
            {
                return char.IsHighSurrogate(_text[index]) && ++index < _text.Length
                    ? char.ConvertToUtf32(_text[index - 1], _text[index])
                    : _text[index];
            }
        }

        private struct StringTextIterator : ITextIterator
        {
            private string _text;

            public int Offset { get; }
            public int Length { get; }

            public StringTextIterator(string text, int offset, int length)
            {
                _text = text ?? throw new ArgumentNullException(nameof(text));
                Offset = offset;
                Length = length;
            }

            public int GetCharacter(ref int index)
            {
                return char.IsHighSurrogate(_text[index]) && ++index < _text.Length
                    ? char.ConvertToUtf32(_text[index - 1], _text[index])
                    : _text[index];
            }
        }

        public struct StringGlyphEnumerable : IEnumerable<Glyph>
        {
            public GlyphEnumerator Glyphs;

            public StringGlyphEnumerable(BitmapFont font,
                string text, int offset, int length, Point2 position)
            {
                Glyphs = new GlyphEnumerator(
                    font, new StringTextIterator(text, offset, length), position);
            }
            
            IEnumerator<Glyph> IEnumerable<Glyph>.GetEnumerator() => Glyphs;
            IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException();
        }

        public struct StringBuilderGlyphEnumerable : IEnumerable<Glyph>
        {
            public GlyphEnumerator Glyphs;

            public StringBuilderGlyphEnumerable(
                BitmapFont font, StringBuilder text, int offset, int length, Point2 position)
            {
                Glyphs = new GlyphEnumerator(
                    font, new StringBuilderTextIterator(text, offset, length), position);
            }

            IEnumerator<Glyph> IEnumerable<Glyph>.GetEnumerator() => Glyphs;

            IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException();
        }

        public struct GlyphEnumerator : IEnumerator<Glyph>
        {
            private BitmapFont _font;
            private ITextIterator _textIterator;
            private Point2 _position;

            private int _index;
            private Vector2 _positionDelta;
            private Glyph? _previousGlyph;
            public Glyph CurrentGlyph;

            // casting a struct to object will box it, a behaviour we want to avoid...
            object IEnumerator.Current => throw new InvalidOperationException();

            public Glyph Current => CurrentGlyph;

            public GlyphEnumerator(BitmapFont font, ITextIterator text, Point2 position)
            {
                _font = font;
                _textIterator = text;
                _position = position;

                _index = -1;
                _positionDelta = Vector2.Zero;
                CurrentGlyph = new Glyph();
                _previousGlyph = null;
            }

            public bool MoveNext()
            {
                _index++;
                if (_index >= _textIterator.Length)
                    return false;

                int character = _textIterator.GetCharacter(ref _index);

                CurrentGlyph.Character = character;
                CurrentGlyph.FontRegion = _font.GetCharacterRegion(character);
                CurrentGlyph.Position = _position + _positionDelta;

                if (CurrentGlyph.FontRegion != null)
                {
                    CurrentGlyph.Position.X += CurrentGlyph.FontRegion.XOffset;
                    CurrentGlyph.Position.Y += CurrentGlyph.FontRegion.YOffset;
                    _positionDelta.X += CurrentGlyph.FontRegion.XAdvance + _font.LetterSpacing;
                }

                if (UseKernings && _previousGlyph?.FontRegion != null)
                {
                    if (_previousGlyph.Value.FontRegion.Kernings.TryGetValue(character, out int amount))
                        _positionDelta.X += amount;
                }

                _previousGlyph = CurrentGlyph;

                if (character != '\n')
                    return true;

                _positionDelta.Y += _font.LineHeight;
                _positionDelta.X = 0;
                _previousGlyph = null;

                return true;
            }

            public void Dispose()
            {

            }

            public void Reset()
            {
                _positionDelta = new Point2();
                _index = -1;
                _previousGlyph = null;
            }
        }
    }
}