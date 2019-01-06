using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public static class TextureAtlasExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
        {
            Draw(spriteBatch, textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0, clippingRectangle);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Rectangle? clippingRectangle = null)
        {
            if (textureRegion.Bounds.IsVisible(ref position, origin, scale, clippingRectangle, out Rectangle srcRect))
                spriteBatch.Draw(textureRegion.Texture, position, srcRect, color, rotation, origin, scale, effects, layerDepth);
        }

        public static bool IsVisible(
            this Rectangle sourceRect, ref Vector2 position,
            Vector2 origin, Vector2 scale, Rectangle? clipRect, out Rectangle clippedRect)
        {
            if (!clipRect.HasValue)
            {
                clippedRect = sourceRect;
                return true;
            }

            float x = position.X - origin.X * scale.X;
            float y = position.Y - origin.Y * scale.Y;
            float width = sourceRect.Width * scale.X;
            float height = sourceRect.Height * scale.Y;
            var dstRect = new RectangleF(x, y, width, height);

            var clippedRectF = ClipSourceRectangle(sourceRect, dstRect, clipRect.Value);
            clippedRect = clippedRectF.ToRectangle();

            position.X += 0.5f + (clippedRect.X - sourceRect.X) * scale.X;
            position.Y += 0.5f + (clippedRect.Y - sourceRect.Y) * scale.Y;
            
            return clippedRectF.Width >= 1 && clippedRectF.Height >= 1f;
        }

        public static void Draw(
            this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
        {
            if (textureRegion is NinePatchRegion2D ninePatchRegion)
                Draw(spriteBatch, ninePatchRegion, destinationRectangle, color, clippingRectangle);
            else
                Draw(spriteBatch, textureRegion.Texture, textureRegion.Bounds, destinationRectangle, color, clippingRectangle);
        }

        public static void Draw(
            this SpriteBatch spriteBatch, NinePatchRegion2D ninePatchRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
        {
            var destinationPatches = ninePatchRegion.CreatePatches(destinationRectangle);
            var sourcePatches = ninePatchRegion.SourcePatches;

            for (var i = 0; i < sourcePatches.Length; i++)
            {
                var sourcePatch = sourcePatches[i];
                var destinationPatch = destinationPatches[i];

                if (clippingRectangle.HasValue)
                {
                    sourcePatch = ClipSourceRectangle(sourcePatch, destinationPatch, clippingRectangle.Value).ToRectangle();
                    destinationPatch = ClipDestinationRectangle(destinationPatch, clippingRectangle.Value).ToRectangle();
                    Draw(spriteBatch, ninePatchRegion.Texture, sourcePatch, destinationPatch, color, clippingRectangle);
                }
                else
                {
                    if (destinationPatch.Width > 0 && destinationPatch.Height > 0)
                        spriteBatch.Draw(ninePatchRegion.Texture, sourceRectangle: sourcePatch, destinationRectangle: destinationPatch, color: color);
                }
            }
        }

        public static void Draw(
            this SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle)
        {
            if (clippingRectangle.HasValue)
            {
                sourceRectangle = ClipSourceRectangle(sourceRectangle, destinationRectangle, clippingRectangle.Value).ToRectangle();
                destinationRectangle = ClipDestinationRectangle(destinationRectangle, clippingRectangle.Value).ToRectangle();
            }

            if (destinationRectangle.Width > 0 && destinationRectangle.Height > 0)
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        private static RectangleF ClipSourceRectangle(RectangleF sourceRectangle, RectangleF destinationRectangle, RectangleF clippingRectangle)
        {
            float left = clippingRectangle.Left - destinationRectangle.Left + 1f;
            float right = destinationRectangle.Right - clippingRectangle.Right + 1f;
            float top = clippingRectangle.Top - destinationRectangle.Top + 1f;
            float bottom = destinationRectangle.Bottom - clippingRectangle.Bottom + 1f;
            float x = left > 0 ? left : 0;
            float y = top > 0 ? top : 0;
            float w = (right > 0 ? right : 0) + x;
            float h = (bottom > 0 ? bottom : 0) + y;

            float scaleX = destinationRectangle.Width / sourceRectangle.Width;
            float scaleY = destinationRectangle.Height / sourceRectangle.Height;
            x /= scaleX;
            y /= scaleY;
            w /= scaleX;
            h /= scaleY;

            return new RectangleF(
                sourceRectangle.X + x,
                sourceRectangle.Y + y,
                sourceRectangle.Width - w + 0.5f, 
                sourceRectangle.Height - h + 0.5f);
        }

        private static RectangleF ClipDestinationRectangle(RectangleF destinationRectangle, RectangleF clippingRectangle)
        {
            float left = clippingRectangle.Left < destinationRectangle.Left ? destinationRectangle.Left : clippingRectangle.Left;
            float top = clippingRectangle.Top < destinationRectangle.Top ? destinationRectangle.Top : clippingRectangle.Top;
            float bottom = clippingRectangle.Bottom < destinationRectangle.Bottom ? clippingRectangle.Bottom : destinationRectangle.Bottom;
            float right = clippingRectangle.Right < destinationRectangle.Right ? clippingRectangle.Right : destinationRectangle.Right;
            return new RectangleF(left, top, right - left, bottom - top);
        }
    }
}