using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;
using Glyph = MonoGame.Extended.BitmapFonts.BitmapFont.Glyph;

namespace MonoGame.Extended.BitmapFonts
{
    public static partial class BitmapFontExtensions
    {
        private static void GetSprites(
            IList<Glyph> glyphs, ICollection<GlyphSprite> output, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, RectangleF? clipRect)
        {
            GlyphSprite sprite;
            for (int i = 0, count = glyphs.Count; i < count; i++)
            {
                Glyph glyph = glyphs[i];
                if (glyph.FontRegion != null)
                {
                    Vector2 glyphOrigin = origin - glyph.Position;
                    Vector2 newPos = position;
                    TextureRegion2D region = glyph.FontRegion.TextureRegion;

                    sprite.Visible = region.Bounds.IsVisible(
                        ref newPos, glyphOrigin, scale, clipRect, out RectangleF srcRect);

                    if (!sprite.Visible) // restore values
                    {
                        newPos = position;
                        srcRect = region.Bounds;
                    }

                    sprite.Char = glyph.Character;
                    sprite.Texture = glyph.FontRegion.TextureRegion.Texture;
                    sprite.Index = i;
                    sprite.SourceRect = srcRect;
                    sprite.Position = newPos;
                    sprite.Color = color;
                    sprite.Rotation = rotation;
                    sprite.Origin = glyphOrigin;
                    sprite.Scale = scale;
                    sprite.Depth = depth;

                    output.Add(sprite);
                }
            }
        }

        public static void GetGlyphSprites(
            IList<Glyph> glyphs, ICollection<GlyphSprite> output, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, RectangleF? clipRect)
        {
            GetSprites(glyphs, output, position, color, rotation, origin, scale, depth, clipRect);
        }

        private static void GetBatchedSprites(
            IList<Glyph> glyphs, ICollection<GlyphBatchedSprite> output, Vector2 position, Color color,
            float rotation, Vector2 origin, Vector2 scale, float depth, RectangleF? clipRect)
        {
            GlyphBatchedSprite sprite;
            for (int i = 0, count = glyphs.Count; i < count; i++)
            {
                Glyph glyph = glyphs[i];
                if (glyph.FontRegion != null)
                {
                    Vector2 glyphOrigin = -glyph.Position + origin;
                    Vector2 newPos = position;
                    TextureRegion2D region = glyph.FontRegion.TextureRegion;

                    sprite.Visible = region.Bounds.IsVisible(
                        ref newPos, glyphOrigin, scale, clipRect, out RectangleF srcRect);

                    if (!sprite.Visible) // restore values
                    {
                        newPos = position;
                        srcRect = region.Bounds;
                    }

                    sprite.Char = glyph.Character;
                    sprite.Index = i;
                    sprite.Texture = glyph.FontRegion.TextureRegion.Texture;
                    sprite.Sprite = default;
                    sprite.Sprite.SetTransform(newPos, rotation, scale, origin, srcRect.Size);
                    sprite.Sprite.SetTexCoords(sprite.Texture.Texel, srcRect);
                    sprite.Sprite.SetDepth(depth);
                    sprite.Sprite.SetColor(color);

                    output.Add(sprite);
                }
            }
        }

        public static void GetGlyphBatchedSprites(
            IList<Glyph> glyphs, ICollection<GlyphBatchedSprite> output, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            GetBatchedSprites(glyphs, output, position, color, rotation, origin, scale, depth, clipRect);
        }

        #region DrawString GlyphBatchedSprite
        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                ref GlyphBatchedSprite s = ref sprites.GetReferenceAt(i);
                if (s.Visible)
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
                if (s.Visible)
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
                if (p.Visible)
                {
                    batch.Draw(
                        p.Texture, p.Position, p.SourceRect, p.Color,
                        p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth);
                }
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
                if (p.Visible)
                {
                    batch.Draw(
                    p.Texture, p.Position + position, p.SourceRect,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth + depth);
                }
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
                if (p.Visible)
                {
                    batch.Draw(
                    p.Texture, p.Position + position, p.SourceRect, p.Color, p.Rotation,
                    p.Origin, p.Scale * scale, SpriteEffects.None, p.Depth + depth);
                }
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> sprites, Vector2 position, Vector2 scale, float depth)
        {
            DrawString(batch, sprites, 0, sprites.Count, position, scale, depth);
        }
        #endregion
    }
}