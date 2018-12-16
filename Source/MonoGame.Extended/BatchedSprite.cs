using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;

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
                    _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Rgba32);
                    _texture.SetData(new[] { Color.White });
                }
                return _texture;
            }
        }
        
        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite sprite, float depth)
        {
            DrawRef(batch, texture, ref sprite, depth);
        }
    
        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite sprite)
        {
            DrawRef(batch, texture, ref sprite);
        }

        public static void DrawRef(this SpriteBatch batch, Texture2D texture, ref BatchedSprite sprite, float depth)
        {
            batch.DrawRef(texture, ref sprite.TL, ref sprite.TR, ref sprite.BL, ref sprite.BR, depth);
        }

        public static void DrawRef(this SpriteBatch batch, Texture2D texture, ref BatchedSprite sprite)
        {
            batch.DrawRef(texture, ref sprite.TL, ref sprite.TR, ref sprite.BL, ref sprite.BR);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite[] sprites, int offset, int count)
        {
            for (int i = offset; i < count; i++)
                batch.DrawRef(texture, ref sprites[i]);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite[] sprites, int offset, int count, float depth)
        {
            for (int i = offset; i < count; i++)
                batch.DrawRef(texture, ref sprites[i], depth);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, IList<BatchedSprite> sprites, int offset, int count)
        {
            for (int i = offset; i < count; i++)
                batch.Draw(texture, sprites[i]);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, IList<BatchedSprite> sprites, int offset, int count, float depth)
        {
            for (int i = offset; i < count; i++)
                batch.Draw(texture, sprites[i], depth);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, IReferenceList<BatchedSprite> sprites, int offset, int count)
        {
            for (int i = offset; i < count; i++)
                batch.DrawRef(texture, ref sprites.GetReferenceAt(i));
        }

        public static void Draw(
            this SpriteBatch batch, Texture2D texture,
            IReferenceList<BatchedSprite> sprites, int offset, int count, float depth)
        {
            for (int i = offset; i < count; i++)
                batch.DrawRef(texture, ref sprites.GetReferenceAt(i), depth);
        }

        public static void SetDepth(this ref BatchedSprite sprite, float depth)
        {
            sprite.TL.Position.Z = depth;
            sprite.TR.Position.Z = depth;
            sprite.BL.Position.Z = depth;
            sprite.BR.Position.Z = depth;
        }

        public static void SetColor(this ref BatchedSprite sprite, ref Color color)
        {
            sprite.TL.Color = color;
            sprite.TR.Color = color;
            sprite.BL.Color = color;
            sprite.BR.Color = color;
        }

        public static void SetTexCoords(this ref BatchedSprite sprite, TextureRegion2D region)
        {
            SetTexCoords(ref sprite, region.Texture.Texel, region.Bounds);
        }

        public static void SetTexCoords(this ref BatchedSprite sprite, Vector2 textureTexel, RectangleF sourceRect)
        {
            SourceRectToTexCoords(textureTexel, sourceRect,
                ref sprite.TL.TextureCoordinate, ref sprite.TR.TextureCoordinate,
                ref sprite.BL.TextureCoordinate, ref sprite.BR.TextureCoordinate);
        }

        public static void SetTransform(this ref BatchedSprite sprite, Matrix2 matrix, Point sourceSize)
        {
            SetTransform(ref sprite, matrix, sourceSize.ToVector2());
        }

        public static void SetTransform(
            this ref BatchedSprite sprite,
            Vector2 position, float rotation, Vector2? scale, Vector2? origin, Vector2 sourceSize)
        {
            var matrix = Matrix2.CreateFrom(position, rotation, scale, origin);
            SetTransform(ref sprite, matrix, sourceSize);
        }

        public static void SetTransform(
            this ref BatchedSprite sprite,
            Vector2 position, float rotation, Vector2? scale, Vector2? origin, Point sourceSize)
        {
            SetTransform(ref sprite, position, rotation, scale, origin, sourceSize.ToVector2());
        }

        public static void SetTransform(this ref BatchedSprite sprite, Matrix2 matrix, Vector2 sourceSize)
        {
            Transform(ref matrix, 0, 0, ref sprite.TL.Position);
            Transform(ref matrix, sourceSize.X, 0, ref sprite.TR.Position);
            Transform(ref matrix, 0, sourceSize.Y, ref sprite.BL.Position);
            Transform(ref matrix, sourceSize.X, sourceSize.Y, ref sprite.BR.Position);
        }

        private static void Transform(ref Matrix2 matrix, float x, float y, ref Vector3 output)
        {
            output.X = x * matrix.M11 + y * matrix.M21 + matrix.M31;
            output.Y = x * matrix.M12 + y * matrix.M22 + matrix.M32;
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

        public static Matrix2 GetMatrixFromRect(
            RectangleF destination, Vector2 origin, float rotation, Point sourceSize)
        {
            return GetMatrixFromRect(destination, origin, rotation, sourceSize.ToVector2());
        }

        public static Matrix2 GetMatrixFromRect(
            RectangleF destination, Vector2 origin, float rotation, Vector2 sourceSize)
        {
            origin.X *= destination.Width / sourceSize.X;
            origin.Y *= destination.Height / sourceSize.Y;

            Vector2 pos = new Vector2(destination.X, destination.Y);
            Vector2 scale = new Vector2(
                destination.Width / sourceSize.X,
                destination.Height / sourceSize.Y);

            return Matrix2.CreateFrom(pos, rotation, scale, origin);
        }

        public static void SourceRectToTexCoords(
            Vector2 textureTexel, RectangleF sourceRect,
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
    }
}