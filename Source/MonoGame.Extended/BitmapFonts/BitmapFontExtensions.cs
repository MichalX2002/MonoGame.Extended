using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Glyph = MonoGame.Extended.BitmapFonts.BitmapFont.Glyph;

namespace MonoGame.Extended.BitmapFonts
{
    public static partial class BitmapFontExtensions
    {
        private static GlyphBatchedSprite GetBatchedSprite(
            Glyph glyph, int index, Vector2 position, Rectangle src, Color color,
            float rotation, Vector2 origin, Vector2? scale, float depth)
        {
            var item = new GlyphBatchedSprite(glyph.Character, index, glyph.FontRegion.TextureRegion.Texture);
            item.Sprite.SetTransform(position, rotation, scale, origin, src.Size);
            item.Sprite.SetTexCoords(item.Texture.Texel, src);
            item.Sprite.SetDepth(depth);
            item.Sprite.SetColor(color);
            return item;
        }
    }
}