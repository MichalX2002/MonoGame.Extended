using System.Text;
using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.BitmapFonts
{
    public partial class BitmapFont
    {
		public SizeF GetGlyphs(ICharIterator iterator, IReferenceList<Glyph> output)
        {
            if (iterator.Count <= 0)
                return SizeF.Empty;
            
            float largestW = 0;
            Vector2 positionDelta = new Vector2(0, 0);
			Glyph previousGlyph = new Glyph();

            for (int i = iterator.Offset; i < iterator.Offset + iterator.Count; i++)
            {
                int character = iterator.GetCharacter(ref i);
                BitmapFontRegion region = GetCharacterRegion(character);
                Vector2 newPos = positionDelta;

                if (region != null)
                {
                    newPos.X += region.XOffset;
                    newPos.Y += region.YOffset;
                    positionDelta.X += region.XAdvance + LetterSpacing;
                }

                if (UseKernings && previousGlyph.FontRegion != null)
                {
                    if (previousGlyph.FontRegion.Kernings.TryGetValue(character, out int amount))
                        positionDelta.X += amount;
                }

                // use previousGlyph to store the new Glyph
                previousGlyph = new Glyph(character, newPos, region);
                output.Add(previousGlyph);

                if (positionDelta.X > largestW)
                    largestW = positionDelta.X;

                if (character == '\n')
                {
                    positionDelta.Y += LineHeight;
                    positionDelta.X = 0;
                    previousGlyph = default;
                }
            }
            
            return new SizeF(largestW, positionDelta.Y + LineHeight);
        }

		public SizeF GetGlyphs(string text, int offset, int count, IReferenceList<Glyph> output)
        {
            using (var iterator = CharIteratorPool.Rent(text, offset, count))
                return GetGlyphs(iterator, output);
        }

        public SizeF GetGlyphs(StringBuilder text, int offset, int count, IReferenceList<Glyph> output)
        {
            using (var iterator = CharIteratorPool.Rent(text, offset, count))
                return GetGlyphs(iterator, output);
        }

        public SizeF GetGlyphs(string text, IReferenceList<Glyph> output)
        {
            return GetGlyphs(text, 0, text.Length, output);
        }

        public SizeF GetGlyphs(StringBuilder text, IReferenceList<Glyph> output)
        {
            return GetGlyphs(text, 0, text.Length, output);
        }

        public SizeF MeasureString(IReferenceList<Glyph> glyphs, int offset, int count)
        {
            if (count <= 0)
                return SizeF.Empty;

            if (offset > glyphs.Count)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (offset + count > glyphs.Count)
                throw new ArgumentException(nameof(count),
                    "There's not enough glyphs for the requested amount " +
                    $"({count} were requested, {glyphs.Count - offset} are available).");

            var size = new SizeF(0, LineHeight);
            int c = offset + count;
            for (int i = offset; i < c; i++)
            {
                ref Glyph glyph = ref glyphs.GetReferenceAt(i);
                if (glyph.FontRegion != null)
                {
                    float right = glyph.Position.X + glyph.FontRegion.Width;
                    if (right > size.Width)
                        size.Width = right;
                }

                if (glyph.Character == '\n')
                    size.Height += LineHeight;
            }
            return size;
        }
        
        public SizeF MeasureString(IReferenceList<Glyph> glyphs)
        {
            return MeasureString(glyphs, 0, glyphs.Count);
        }
    }
}
