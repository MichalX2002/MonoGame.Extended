using System.Text;
using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public partial class BitmapFont
    {
		public SizeF GetGlyphs(ITextIterator iterator, IReferenceList<Glyph> output)
        {
            Vector2 positionDelta = Vector2.Zero;
			Glyph previousGlyph = default;

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

                Glyph glyph = new Glyph(character, newPos, region);
                output.AddRef(ref glyph);

                if (UseKernings && previousGlyph.FontRegion != null)
                {
                    if (previousGlyph.FontRegion.Kernings.TryGetValue(character, out int amount))
                        positionDelta.X += amount;
                }

                if (character == '\n')
                {
                    positionDelta.Y += LineHeight;
                    positionDelta.X = 0;
                    previousGlyph = default;
                }
                else
                {
                    previousGlyph = glyph;
                }
            }

            positionDelta.Y += LineHeight;
            return positionDelta;
        }

		public SizeF GetGlyphs(string text, int offset, int length, IReferenceList<Glyph> output)
        {
            return GetGlyphs(new StringTextIterator(text, offset, length), output);
        }

        public SizeF GetGlyphs(StringBuilder text, int offset, int length, IReferenceList<Glyph> output)
        {
            return GetGlyphs(new StringBuilderTextIterator(text, offset, length), output);
        }

        public SizeF GetGlyphs(string text, IReferenceList<Glyph> output)
        {
            return GetGlyphs(text, 0, text.Length, output);
        }

        public SizeF GetGlyphs(StringBuilder text, IReferenceList<Glyph> output)
        {
            return GetGlyphs(text, 0, text.Length, output);
        }

        public SizeF MeasureString(IReferenceList<Glyph> glyphs, int offset, int length)
        {
            if (length <= 0)
                return SizeF.Empty;

            if (offset > glyphs.Count)
                throw new ArgumentOutOfRangeException(nameof(offset));
            
            if (offset + length > glyphs.Count)
                throw new ArgumentException(nameof(length),
                    "There's not enough glyphs for the requested amount " +
                    $"({length} were requested, {glyphs.Count - offset} are available).");

            var size = new SizeF(0, LineHeight);
            int count = offset + length;
            for (int i = offset; i < count; i++)
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
