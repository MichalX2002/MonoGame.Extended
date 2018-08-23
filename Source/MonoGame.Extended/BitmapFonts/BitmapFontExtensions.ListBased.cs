using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Glyph = MonoGame.Extended.BitmapFonts.BitmapFont.Glyph;

namespace MonoGame.Extended.BitmapFonts
{
    public static partial class BitmapFontExtensions
    {
        public static void GetGlyphSprites(
            IReferenceList<Glyph> glyphs, IReferenceList<GlyphBatchedSprite> output, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            GetBaseSprites(glyphs, output, position, color, rotation, origin, scale, depth, clipRect, GetSprite);
        }

        public static void GetGlyphPositions(
            IReferenceList<Glyph> glyphs, IReferenceList<GlyphSprite> output, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            GetBaseSprites(glyphs, output, position, color, rotation, origin, scale, depth, clipRect, GetPos);
        }

        private static void GetBaseSprites<T>(
            IReferenceList<Glyph> glyphs, IReferenceList<T> output,
            Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale,
            float depth, Rectangle? clipRect, GetSpriteDelegate<T> getSprite)
        {
            for (int i = 0, count = glyphs.Count; i < count; i++)
            {
                ref Glyph glyph = ref glyphs.GetReferenceAt(i);
                if (glyph.FontRegion != null)
                {
                    Vector2 glyphOrigin = -glyph.Position + origin;
                    Rectangle srcRect = glyph.FontRegion.TextureRegion.Bounds;
                    Vector2 newPos = position;

                    if (srcRect.IsVisible(ref newPos, glyphOrigin, scale, clipRect, out srcRect))
                    {
                        output.AddRef(getSprite.Invoke(
                            glyph, i, newPos, srcRect, color, rotation, glyphOrigin, scale, depth));
                    }
                }
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites)
        {
            for (int i = 0, count = sprites.Count; i < count; i++)
            {
                ref GlyphBatchedSprite s = ref sprites.GetReferenceAt(i);
                batch.Draw(s.Texture, s.Sprite);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites, float depth)
        {
            for (int i = 0, count = sprites.Count; i < count; i++)
            {
                ref GlyphBatchedSprite s = ref sprites.GetReferenceAt(i);
                batch.Draw(s.Texture, s.Sprite, depth);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphSprite> positions)
        {
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphSprite> positions, float depth)
        {
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth + depth);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> positions, Vector2 position, float depth)
        {
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position + position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth + depth);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> positions, Vector2 position, Vector2 scale, float depth)
        {
            for (int i = 0, count = positions.Count; i < count; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position + position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale + scale, SpriteEffects.None, p.Depth + depth);
            }
        }
    }
}
