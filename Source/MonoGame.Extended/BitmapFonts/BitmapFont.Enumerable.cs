using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public partial class BitmapFont
    {

        public RectangleF GetStringRectangle(string text, Vector2 position)
        {
            return GetStringRectangle(text, 0, text.Length, position);
        }

        public RectangleF GetStringRectangle(
            string text, int offset, int count, Vector2 position)
        {
            using (var enumerator = GetGlyphs(text, offset, count, position))
                return GetStringRectangle(enumerator, position);
        }

        public SizeF MeasureString(string text, int offset, int count)
        {
            if (string.IsNullOrEmpty(text))
                return SizeF.Empty;

            return GetStringRectangle(text, offset, count, PointF.Zero).Size;
        }

        public SizeF MeasureString(string text)
        {
            return MeasureString(text, 0, text.Length);
        }

        public RectangleF GetStringRectangle(StringBuilder text, Vector2 position)
        {
            return GetStringRectangle(text, 0, text.Length, position);
        }

        public RectangleF GetStringRectangle(StringBuilder text, int offset, int count, Vector2 position)
        {
            using (var enumerator = GetGlyphs(text, offset, count, position))
                return GetStringRectangle(enumerator, position);
        }

        public SizeF MeasureString(StringBuilder text, int offset, int count)
        {
            if (text == null || text.Length == 0)
                return SizeF.Empty;

            return GetStringRectangle(text, offset, count, PointF.Zero).Size;
        }

        public SizeF MeasureString(StringBuilder text)
        {
            return MeasureString(text, 0, text.Length);
        }

        public RectangleF GetStringRectangle(IEnumerator<Glyph> iterator, Vector2 position)
        {
            var rectangle = new RectangleF(position.X, position.Y, 0, LineHeight);
            while (iterator.MoveNext())
            {
                Glyph glyph = iterator.Current;
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

        public IEnumerator<Glyph> GetGlyphs(ICharIterator text, Vector2 position = default)
        {
            return GlyphEnumeratorPool.Rent(this, text, position);
        }

        public IEnumerator<Glyph> GetGlyphs(string text, int offset, int count, Vector2 position = default)
        {
            var iterator = CharIteratorPool.Rent(text, offset, count);
            return GlyphEnumeratorPool.Rent(this, iterator, position);
        }

        public IEnumerator<Glyph> GetGlyphs(string text, Vector2 position = default)
        {
            return GetGlyphs(text, 0, text.Length, position);
        }

        public IEnumerator<Glyph> GetGlyphs(StringBuilder text, int offset, int count, Vector2 position = default)
        {
            var iterator = CharIteratorPool.Rent(text, offset, count);
            return GlyphEnumeratorPool.Rent(this, iterator, position);
        }

        public IEnumerator<Glyph> GetGlyphs(StringBuilder text, Vector2 position = default)
        {
            return GetGlyphs(text, 0, text.Length, position);
        }
    }
}
