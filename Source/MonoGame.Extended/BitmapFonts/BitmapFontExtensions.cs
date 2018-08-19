using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Glyph = MonoGame.Extended.BitmapFonts.BitmapFont.Glyph;

namespace MonoGame.Extended.BitmapFonts
{
    public static class BitmapFontExtensions
    {
        private delegate T GetPositionDelegate<T>(
            Glyph glyph, int index, Vector2 position, Rectangle sourceRect, Color color,
            float rotation, Vector2 origin, Vector2 scale, float depth);
        
        private static void ThrowOnArgs(SpriteEffects effect)
        {
            if (effect != SpriteEffects.None)
                throw new NotSupportedException($"{effect} is not currently supported for {nameof(BitmapFont)}");
        }

        /// <summary>
        ///     Adds a string to a batch of sprites for rendering using the specified font,
        ///     text, position, color, rotation, origin, scale, effects and layer.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font">A font for displaying text.</param>
        /// <param name="text">The text message to display.</param>
        /// <param name="position">The location (in screen coordinates) to draw the text.</param>
        /// <param name="color">The <see cref="Color" /> to tint a sprite.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the text about its origin.</param>
        /// <param name="origin">The origin for each letter; the default is (0,0) which is the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effect">Effects to apply.</param>
        /// <param name="layerDepth">
        ///     The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer.
        ///     Use SpriteSortMode if you want sprites to be sorted during drawing.
        /// </param>
        /// <param name="clippingRectangle">
        /// Clips the boundaries of the text so that it's not drawn outside the clipping rectangle.
        /// </param>
        public static void DrawString(
            this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position, Color color, float rotation,
            Vector2 origin, Vector2 scale, SpriteEffects effect, float layerDepth, Rectangle? clippingRectangle = null)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            ThrowOnArgs(effect);

            var glyphs = font.GetGlyphs(text, position);
            DrawString(spriteBatch, ref glyphs.Glyphs, position, color,
                rotation, origin, scale, layerDepth, clippingRectangle);
        }

        public static void DrawString(
            this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position, Color color, float rotation,
            Vector2 origin, Vector2 scale, SpriteEffects effect, float layerDepth, Rectangle? clippingRectangle = null)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            ThrowOnArgs(effect);

            var glyphs = font.GetGlyphs(text, position);
            DrawString(spriteBatch, ref glyphs.Glyphs, position, color,
                rotation, origin, scale, layerDepth, clippingRectangle);
        }

        private static void DrawString(SpriteBatch batch, ref BitmapFont.GlyphEnumerator glyphs, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            while (glyphs.MoveNext())
            {
                ref Glyph glyph = ref glyphs.CurrentGlyph;
                if (glyph.FontRegion == null)
                    continue;

                Vector2 characterOrigin = position - glyph.Position + origin;

                batch.Draw(glyph.FontRegion.TextureRegion, position, color,
                    rotation, characterOrigin, scale, SpriteEffects.None, depth, clipRect);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites)
        {
            for (int i = 0, length = sprites.Count; i < length; i++)
            {
                ref GlyphBatchedSprite s = ref sprites.GetReferenceAt(i);
                batch.Draw(s.Texture, s.Sprite);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphBatchedSprite> sprites, float depth)
        {
            for (int i = 0, length = sprites.Count; i < length; i++)
            {
                ref GlyphBatchedSprite s = ref sprites.GetReferenceAt(i);
                batch.Draw(s.Texture, s.Sprite, depth);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphSprite> positions)
        {
            for (int i = 0, length = positions.Count; i < length; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth);
            }
        }

        public static void DrawString(this SpriteBatch batch, IReferenceList<GlyphSprite> positions, float depth)
        {
            for (int i = 0, length = positions.Count; i < length; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth + depth);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> positions, Vector2 position, float depth)
        {
            for (int i = 0, length = positions.Count; i < length; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position + position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale, SpriteEffects.None, p.Depth + depth);
            }
        }

        public static void DrawString(
            this SpriteBatch batch, IReferenceList<GlyphSprite> positions, Vector2 position, Vector2 scale, float depth)
        {
            for (int i = 0, length = positions.Count; i < length; i++)
            {
                ref GlyphSprite p = ref positions.GetReferenceAt(i);
                batch.Draw(p.Texture, p.Position + position, p.SourceRectangle,
                    p.Color, p.Rotation, p.Origin, p.Scale + scale, SpriteEffects.None, p.Depth + depth);
            }
        }

        public static void GetGlyphSprites(
            this BitmapFont font, IList<GlyphBatchedSprite> output, string text, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            var glyphs = font.GetGlyphs(text, position);
            GetBaseSprites(ref glyphs.Glyphs, output, position, color, rotation, origin, scale, depth, clipRect, GetSprite);
        }

        public static void GetGlyphSprites(
            this BitmapFont font, IList<GlyphBatchedSprite> output, StringBuilder text, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            var glyphs = font.GetGlyphs(text, position);
            GetBaseSprites(ref glyphs.Glyphs, output, position, color, rotation, origin, scale, depth, clipRect, GetSprite);
        }

        public static void GetGlyphPositions(
            this BitmapFont font, IList<GlyphSprite> output, string text, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            var glyphs = font.GetGlyphs(text, position);
            GetBaseSprites(ref glyphs.Glyphs, output, position, color, rotation, origin, scale, depth, clipRect, GetPos);
        }

        public static void GetGlyphPositions(
            this BitmapFont font, IList<GlyphSprite> output, StringBuilder text, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float depth, Rectangle? clipRect)
        {
            var glyphs = font.GetGlyphs(text, position);
            GetBaseSprites(ref glyphs.Glyphs, output, position, color, rotation, origin, scale, depth, clipRect, GetPos);
        }

        private static GlyphBatchedSprite GetSprite(
            Glyph glyph, int index, Vector2 position, Rectangle src, Color color,
            float rotation, Vector2 origin, Vector2 scale, float depth)
        {
            var item = new GlyphBatchedSprite
            {
                Char = glyph.Character,
                Texture = glyph.FontRegion.TextureRegion.Texture,
                Index = index,
            };
            
            item.Sprite.SetTransform(position, rotation, scale, origin, src.Size);
            item.Sprite.SetTexCoords(item.Texture.Texel, src);
            item.Sprite.SetDepth(depth);
            item.Sprite.SetColor(color);
            return item;
        }

        private static GlyphSprite GetPos(
            Glyph glyph, int index, Vector2 position, Rectangle src, Color color,
            float rotation, Vector2 origin, Vector2 scale, float depth)
        {
            return new GlyphSprite(
                glyph.Character, glyph.FontRegion.TextureRegion.Texture,
                index, src, position, color, rotation, origin, scale, depth);
        }

        private static void GetBaseSprites<T>(
            ref BitmapFont.GlyphEnumerator glyphs, IList<T> output,
            Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale,
            float depth, Rectangle? clipRect, GetPositionDelegate<T> onItem)
        {
            int index = 0;
            while (glyphs.MoveNext())
            {
                ref Glyph glyph = ref glyphs.CurrentGlyph;
                if (glyph.FontRegion != null)
                {
                    Vector2 glyphOrigin = position - glyph.Position + origin;
                    Rectangle srcRect = glyph.FontRegion.TextureRegion.Bounds;
                    Vector2 newPos = position;

                    if (srcRect.IsVisible(ref newPos, glyphOrigin, scale, clipRect, out srcRect))
                    {
                        T item = onItem.Invoke(
                            glyph, index, newPos, srcRect, color, rotation, glyphOrigin, scale, depth);
                        index++;

                        output.Add(item);
                    }
                }
            }
        }

        /// <summary>
        ///     Adds a string to a batch of sprites for rendering using the specified font, text, position, color,
        ///     rotation, origin, scale, effects and layer.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font">A font for displaying text.</param>
        /// <param name="text">The text message to display.</param>
        /// <param name="position">The location (in screen coordinates) to draw the text.</param>
        /// <param name="color">
        /// The <see cref="Color" /> to tint a sprite. Use <see cref="Color.White" /> for full color with no tinting.
        /// </param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the text about its origin.</param>
        /// <param name="origin">The origin for each letter; the default is (0,0) which is the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effect">Effects to apply.</param>
        /// <param name="layerDepth">
        ///     The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer.
        ///     Use SpriteSortMode if you want sprites to be sorted during drawing.
        /// </param>
        /// <param name="clippingRectangle">
        /// Clips the boundaries of the text so that it's not drawn outside the clipping rectangle.
        /// </param>
        public static void DrawString(
            this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position, Color color, float rotation,
            Vector2 origin, float scale, SpriteEffects effect, float layerDepth, Rectangle? clippingRectangle = null)
        {
            DrawString(spriteBatch, font, text, position, color, rotation, origin,
                new Vector2(scale, scale), effect, layerDepth, clippingRectangle);
        }

        public static void DrawString(
            this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position, Color color, float rotation,
            Vector2 origin, float scale, SpriteEffects effect, float layerDepth, Rectangle? clippingRectangle = null)
        {
            DrawString(spriteBatch, font, text, position, color, rotation, origin,
                new Vector2(scale, scale), effect, layerDepth, clippingRectangle);
        }

        /// <summary>
        ///     Adds a string to a batch of sprites for rendering using the specified font, text, position, color,
        ///     layer, and width (in pixels) where to wrap the text at.
        /// </summary>
        /// <remarks>
        ///     <see cref="BitmapFont" /> objects are loaded from the Content Manager.
        ///     See the <see cref="BitmapFont"/> class for more information.
        ///     Before any calls to DrawString you must call <see cref="SpriteBatch.Begin" />. Once all calls 
        ///     are complete, call <see cref="SpriteBatch.End" />.
        ///     Use a newline character (\n) to draw more than one line of text.
        /// </remarks>
        /// <param name="spriteBatch"></param>
        /// <param name="font">A font for displaying text.</param>
        /// <param name="text">The text message to display.</param>
        /// <param name="position">The location (in screen coordinates) to draw the text.</param>
        /// <param name="color">
        ///     The <see cref="Color" /> to tint a sprite. Use <see cref="Color.White" /> for full color with no
        ///     tinting.
        /// </param>
        /// <param name="layerDepth">
        ///     The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer.
        ///     Use SpriteSortMode if you want sprites to be sorted during drawing.
        /// </param>
        /// <param name="clippingRectangle">
        /// Clips the boundaries of the text so that it's not drawn outside the clipping rectangle.
        /// </param>
        public static void DrawString(
            this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position,
            Color color, float layerDepth, Rectangle? clippingRectangle = null)
        {
            DrawString(spriteBatch, font, text, position, color, 0, Vector2.Zero,
                Vector2.One, SpriteEffects.None, layerDepth, clippingRectangle);
        }

        /// <summary>
        ///     Adds a string to a batch of sprites for rendering using the specified font, text, position, color,
        ///     and width (in pixels) where to wrap the text at. The text is drawn on layer 0f.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font">A font for displaying text.</param>
        /// <param name="text">The text message to display.</param>
        /// <param name="position">The location (in screen coordinates) to draw the text.</param>
        /// <param name="color">
        ///     The <see cref="Color" /> to tint a sprite. Use <see cref="Color.White" /> for full color with no
        ///     tinting.
        /// </param>
        /// <param name="clippingRectangle">
        /// Clips the boundaries of the text so that it's not drawn outside the clipping rectangle.
        /// </param>
        public static void DrawString(
            this SpriteBatch spriteBatch, BitmapFont font, string text,
            Vector2 position, Color color, Rectangle? clippingRectangle = null)
        {
            DrawString(spriteBatch, font, text, position, color, 0, Vector2.Zero,
                Vector2.One, SpriteEffects.None, 0, clippingRectangle);
        }

        public static void DrawString(
            this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text,
            Vector2 position, Color color, Rectangle? clippingRectangle = null)
        {
            DrawString(spriteBatch, font, text, position, color, 0, Vector2.Zero,
                Vector2.One, SpriteEffects.None, 0, clippingRectangle);
        }
    }
}