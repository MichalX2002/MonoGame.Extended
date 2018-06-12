using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended
{
    public static class BatchedSpriteExtensions
    {
        private static readonly object _textureCreationLock = new object();
        private static Texture2D _texture;
        
        public static Texture2D GetOnePixelTexture(SpriteBatch spriteBatch)
        {
            lock (_textureCreationLock)
            {
                if (_texture == null)
                {
                    _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    _texture.SetData(new[] { Color.White });
                }
            }

            return _texture;
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, in BatchedSprite sprite, float depth)
        {
            batch.Draw(texture, sprite.TL, sprite.TR, sprite.BL, sprite.BR, depth);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, in BatchedSprite sprite)
        {
            batch.Draw(texture, sprite.TL, sprite.TR, sprite.BL, sprite.BR);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite[] sprites)
        {
            for (int i = 0, length = sprites.Length; i < length; i++)
            {
                batch.Draw(texture, sprites[i]);
            }
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite[] sprites, float depth)
        {
            for (int i = 0, length = sprites.Length; i < length; i++)
            {
                batch.Draw(texture, sprites[i], depth);
            }
        }
    }

    public struct BatchedSprite
    {
        public VertexPositionColorTexture TL;
        public VertexPositionColorTexture TR;
        public VertexPositionColorTexture BL;
        public VertexPositionColorTexture BR;

        public BatchedSprite(
            VertexPositionColorTexture vertexTL, VertexPositionColorTexture vertexTR,
            VertexPositionColorTexture vertexBL, VertexPositionColorTexture vertexBR)
        {
            TL = vertexTL;
            TR = vertexTR;
            BL = vertexBL;
            BR = vertexBR;
        }

        public void SetDepth(float depth)
        {
            TL.Position.Z = depth;
            TR.Position.Z = depth;
            BL.Position.Z = depth;
            BR.Position.Z = depth;
        }

        public void SetColor(in Color color)
        {
            TL.Color = color;
            TR.Color = color;
            BL.Color = color;
            BR.Color = color;
        }

        public void SetTransform(in Matrix2D matrix, in Point sourceSize)
        {
            Transform(matrix, 0, 0, ref TL.Position);
            Transform(matrix, sourceSize.X, 0, ref TR.Position);
            Transform(matrix, 0, sourceSize.Y, ref BL.Position);
            Transform(matrix, sourceSize.X, sourceSize.Y, ref BR.Position);
        }

        public static Matrix2D GetMatrixFromRect(
            in Rectangle destination, Vector2 origin, float rotation, in Point sourceSize)
        {
            origin.X *= destination.Width / (float)sourceSize.X;
            origin.Y *= destination.Height / (float)sourceSize.Y;

            Vector2 pos = new Vector2(destination.X, destination.Y);
            Vector2 size = new Vector2(
                destination.Width / sourceSize.X,
                destination.Height / sourceSize.Y);

            return Matrix2D.CreateFrom(pos, rotation, size, origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Transform(in Matrix2D matrix, float x, float y, ref Vector3 output)
        {
            output.X = x * matrix.M11 + y * matrix.M21 + matrix.M31;
            output.Y = x * matrix.M12 + y * matrix.M22 + matrix.M32;
        }
        
        public void SetTexCoords(TextureRegion2D region)
        {
            SetTexCoords(region.Texture.Texel, region.Bounds);
        }

        public void SetTexCoords(in Vector2 textureTexel, in Rectangle sourceRect)
        {
            SourceRectToTexCoords(textureTexel, sourceRect,
                ref TL.TextureCoordinate, ref TR.TextureCoordinate,
                ref BL.TextureCoordinate, ref BR.TextureCoordinate);
        }

        /*
        public static Vector4 SourceRectToTexCoords(Vector2 textureTexel, Rectangle sourceRect)
        {
            Vector4 output = Vector4.Zero;
            SourceRectToTexCoords(textureTexel, sourceRect, ref output);
            return output;
        }

        public static void SourceRectToTexCoords(Vector2 textureTexel, Rectangle sourceRect, ref Vector4 output)
        {
            output.X = sourceRect.X * textureTexel.X;
            output.Y = sourceRect.Y * textureTexel.Y;
            output.Z = (sourceRect.X + sourceRect.Width) * textureTexel.X;
            output.W = (sourceRect.Y + sourceRect.Height) * textureTexel.Y;
        }
        */

        public static void SourceRectToTexCoords(in Vector2 textureTexel, in Rectangle sourceRect,
            ref Vector2 tl, ref Vector2 tr, ref Vector2 bl, ref Vector2 br)
        {
            float x = sourceRect.X * textureTexel.X;
            float y = sourceRect.Y * textureTexel.Y;
            float z = (sourceRect.X + sourceRect.Width) * textureTexel.X;
            float w = (sourceRect.Y + sourceRect.Height) * textureTexel.Y;

            tl.X = x;   // x
            tl.Y = y;   // y

            tr.X = z;   // z
            tr.Y = y;   // y

            bl.X = x;   // x
            bl.Y = w;   // w

            br.X = z;   // z
            br.Y = w;   // w
            
            /*
            TL.TextureCoordinate.X = _vec.X;
            TL.TextureCoordinate.Y = _vec.Y;

            TR.TextureCoordinate.X = _vec.Z;
            TR.TextureCoordinate.Y = _vec.Y;

            BL.TextureCoordinate.X = _vec.X;
            BL.TextureCoordinate.Y = _vec.W;

            BR.TextureCoordinate.X = _vec.Z;
            BR.TextureCoordinate.Y = _vec.W;
            */
        }
    }
}
