using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public partial class BitmapFont
    {
        public SizeF MeasureString(string text, int offset, int length)
        {
            if (string.IsNullOrEmpty(text))
                return SizeF.Empty;

            return GetStringRectangle(text, offset, length, PointF.Zero).Size;
        }

        public SizeF MeasureString(StringBuilder text, int offset, int length)
        {
            if (text == null || text.Length == 0)
                return SizeF.Empty;

            return GetStringRectangle(text, offset, length, PointF.Zero).Size;
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
            string text, int offset, int length, PointF position)
        {
            var enumerable = GetGlyphs(text, offset, length, position);
            return GetStringRectangle(enumerable.Glyphs, position);
        }

        public RectangleF GetStringRectangle(
            StringBuilder text, int offset, int length, PointF position)
        {
            var enumerable = GetGlyphs(text, offset, length, position);
            return GetStringRectangle(enumerable.Glyphs, position);
        }

        public RectangleF GetStringRectangle(string text, PointF position)
        {
            return GetStringRectangle(text, 0, text.Length, position);
        }

        public RectangleF GetStringRectangle(StringBuilder text, PointF position)
        {
            return GetStringRectangle(text, 0, text.Length, position);
        }

        private RectangleF GetStringRectangle(GlyphEnumerator iterator, PointF position)
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
            string text, int offset, int length, PointF position)
        {
            return new StringGlyphEnumerable(this, text, offset, length, position);
        }

        public StringBuilderGlyphEnumerable GetGlyphs(
            StringBuilder text, int offset, int length, PointF position)
        {
            return new StringBuilderGlyphEnumerable(this, text, offset, length, position);
        }

        public StringGlyphEnumerable GetGlyphs(string text, PointF position)
        {
            return GetGlyphs(text, 0, text.Length, position);
        }

        public StringBuilderGlyphEnumerable GetGlyphs(StringBuilder text, PointF position)
        {
            return GetGlyphs(text, 0, text.Length, position);
        }

        public StringGlyphEnumerable GetGlyphs(string text)
        {
            return GetGlyphs(text, PointF.Zero);
        }

        public StringBuilderGlyphEnumerable GetGlyphs(StringBuilder text)
        {
            return GetGlyphs(text, PointF.Zero);
        }

        public struct StringGlyphEnumerable : IEnumerable<Glyph>
        {
            public GlyphEnumerator Glyphs;

            public StringGlyphEnumerable(BitmapFont font,
                string text, int offset, int length, PointF position)
            {
                Glyphs = new GlyphEnumerator(
                    font, new StringCharIterator(text, offset, length), position);
            }

            IEnumerator<Glyph> IEnumerable<Glyph>.GetEnumerator() => Glyphs;
            IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException();
        }

        public struct StringBuilderGlyphEnumerable : IEnumerable<Glyph>
        {
            public GlyphEnumerator Glyphs;

            public StringBuilderGlyphEnumerable(
                BitmapFont font, StringBuilder text, int offset, int length, PointF position)
            {
                Glyphs = new GlyphEnumerator(
                    font, new StringBuilderCharIterator(text, offset, length), position);
            }

            IEnumerator<Glyph> IEnumerable<Glyph>.GetEnumerator() => Glyphs;
            IEnumerator IEnumerable.GetEnumerator() => throw new InvalidOperationException();
        }

        public struct GlyphEnumerator : IEnumerator<Glyph>
        {
            private BitmapFont _font;
            private ICharIterator _textIterator;
            private PointF _position;

            private int _index;
            private Vector2 _positionDelta;
            private Glyph? _previousGlyph;
            public Glyph CurrentGlyph;

            // casting a struct to object will box it, a behaviour we want to avoid...
            object IEnumerator.Current => throw new InvalidOperationException();

            public Glyph Current => CurrentGlyph;

            public GlyphEnumerator(BitmapFont font, ICharIterator text, PointF position)
            {
                _font = font;
                _textIterator = text;
                _position = position;

                _index = _textIterator.Offset - 1;
                _positionDelta = Vector2.Zero;
                CurrentGlyph = new Glyph();
                _previousGlyph = null;
            }

            public bool MoveNext()
            {
                _index++;
                if (_index >= _textIterator.Offset + _textIterator.Count)
                    return false;

                int character = _textIterator.GetCharacter(ref _index);
                BitmapFontRegion region = _font.GetCharacterRegion(character);
                Vector2 newPos = _position + _positionDelta;

                if (region != null)
                {
                    newPos.X += region.XOffset;
                    newPos.Y += region.YOffset;
                    _positionDelta.X += region.XAdvance + _font.LetterSpacing;
                }

                CurrentGlyph = new Glyph(character, newPos, region);

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

            public void Reset()
            {
                _positionDelta = new PointF();
                _index = -1;
                _previousGlyph = null;
            }

            public void Dispose()
            {
            }
        }
    }
}
