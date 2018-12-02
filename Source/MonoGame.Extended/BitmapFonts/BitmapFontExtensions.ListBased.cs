using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.TextureAtlases;
using Glyph = MonoGame.Extended.BitmapFonts.BitmapFont.Glyph;

namespace MonoGame.Extended.BitmapFonts
{
    public static partial class BitmapFontExtensions
    {
        public static void GetGlyphBatchedSprites(
            IReferenceList<Glyph> glyphs, IReferenceList<GlyphBatchedSprite> output, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            GetBatchedSprites(glyphs, output, position, color, rotation, origin, scale, depth, clipRect);
        }

        public static void GetGlyphSprites(
            IReferenceList<Glyph> glyphs, IReferenceList<GlyphSprite> output, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            GetSprites(glyphs, output, position, color, rotation, origin, scale, depth, clipRect);
        }

        private static void GetSprites(
            IReferenceList<Glyph> glyphs, IReferenceList<GlyphSprite> output, Vector2 position, Color color,
            float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
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
                        output.Add(new GlyphSprite
                        {
                            Char = glyph.Character,
                            Texture = glyph.FontRegion.TextureRegion.Texture,
                            Index = i,
                            SourceRect = srcRect,
                            Position = newPos,
                            Color = color,
                            Rotation = rotation,
                            Origin = glyphOrigin,
                            Scale = scale,
                            Depth = depth
                        });
                    }
                }
            }
        }

        private static void GetBatchedSprites(
            IReferenceList<Glyph> glyphs, IReferenceList<GlyphBatchedSprite> output, Vector2 position, Color color,
            float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            for (int i = 0, count = glyphs.Count; i < count; i++)
            {
                Glyph glyph = glyphs[i];
                if (glyph.FontRegion != null)
                {
                    Vector2 glyphOrigin = -glyph.Position + origin;
                    Rectangle srcRect = glyph.FontRegion.TextureRegion.Bounds;
                    Vector2 newPos = position;

                    if (srcRect.IsVisible(ref newPos, glyphOrigin, scale, clipRect, out srcRect))
                    {
                        output.Add(GetBatchedSprite(glyph, i, newPos, srcRect, color, rotation, glyphOrigin, scale, depth));
                    }
                }
            }
        }

        #region DrawString GlyphBatchedSprite
        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                ref GlyphBatchedSprite s = ref sprites.GetReferenceAt(i);
                batch.DrawRef(s.Texture, ref s.Sprite);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites)
        {
            DrawString(batch, sprites, 0, sprites.Count);
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites, int offset, int count, float depth)
        {
            for (int i = offset; i < count; i++)
            {
                ref GlyphBatchedSprite s = ref sprites.GetReferenceAt(i);
                batch.DrawRef(s.Texture, ref s.Sprite, depth);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites, float depth)
        {
            DrawString(batch, sprites, 0, sprites.Count, depth);
        }
        #endregion

        #region DrawString GlyphSprite
        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphSprite> sprites, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                ref GlyphSprite p = ref sprites.GetReferenceAt(i);
                batch.Draw(
                    p.Texture, p.Position, p.SourceRect, p.Color,
                    p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphSprite> sprites)
        {
            DrawString(batch, sprites, 0, sprites.Count);
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> sprites, int offset, int count, Vector2 position, float depth)
        {
            for (int i = offset; i < count; i++)
            {
                ref GlyphSprite p = ref sprites.GetReferenceAt(i);
                batch.Draw(
                    p.Texture, p.Position + position, p.SourceRect,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth + depth);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> sprites, int offset, int count, Vector2 position)
        {
            DrawString(batch, sprites, offset, count, position, 0);
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> sprites, Vector2 position, float depth)
        {
            DrawString(batch, sprites, 0, sprites.Count, position, depth);
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphSprite> sprites, Vector2 position)
        {
            DrawString(batch, sprites, position, 0);
        }
        
        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> sprites,
            int offset, int count, Vector2 position, Vector2 scale, float depth)
        {
            for (int i = offset; i < count; i++)
            {
                ref GlyphSprite p = ref sprites.GetReferenceAt(i);
                batch.Draw(
                    p.Texture, p.Position + position, p.SourceRect, p.Color, p.Rotation,
                    p.Origin, p.Scale * scale, SpriteEffects.None, p.Depth + depth);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> sprites, Vector2 position, Vector2 scale, float depth)
        {

        }
        #endregion
    }
}
