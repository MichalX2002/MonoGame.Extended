using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

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

        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite[] sprites)
        {
            for (int i = 0; i < sprites.Length; i++)
                batch.DrawRef(texture, ref sprites[i]);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, BatchedSprite[] sprites, float depth)
        {
            for (int i = 0; i < sprites.Length; i++)
                batch.DrawRef(texture, ref sprites[i], depth);
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, IReferenceList<BatchedSprite> sprites)
        {
            for (int i = 0; i < sprites.Count; i++)
                batch.DrawRef(texture, ref sprites.GetReferenceAt(i));
        }

        public static void Draw(this SpriteBatch batch, Texture2D texture, IReferenceList<BatchedSprite> sprites, float depth)
        {
            for (int i = 0; i < sprites.Count; i++)
                batch.DrawRef(texture, ref sprites.GetReferenceAt(i), depth);
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
        
        public void SetColor(Color color)
        {
            TL.Color = color;
            TR.Color = color;
            BL.Color = color;
            BR.Color = color;
        }

        public void SetTransform(Matrix2 matrix, Vector2 sourceSize)
        {
            Transform(matrix, 0, 0, ref TL.Position);
            Transform(matrix, sourceSize.X, 0, ref TR.Position);
            Transform(matrix, 0, sourceSize.Y, ref BL.Position);
            Transform(matrix, sourceSize.X, sourceSize.Y, ref BR.Position);
        }

        public void SetTransform(Matrix2 matrix, Point sourceSize)
        {
            SetTransform(matrix, sourceSize.ToVector2());
        }

        public void SetTransform(
            Vector2 position, float rotation, Vector2 scale, Vector2 origin, Vector2 sourceSize)
        {
            var matrix = Matrix2.CreateFrom(position, rotation, scale, origin);
            SetTransform(matrix, sourceSize);
        }

        public void SetTransform(
            Vector2 position, float rotation, Vector2 scale, Vector2 origin, Point sourceSize)
        {
            SetTransform(position, rotation, scale, origin, sourceSize.ToVector2());
        }

        private void Transform(Matrix2 matrix, float x, float y, ref Vector3 output)
        {
            output.X = x * matrix.M11 + y * matrix.M21 + matrix.M31;
            output.Y = x * matrix.M12 + y * matrix.M22 + matrix.M32;
        }
        
        public void SetTexCoords(TextureRegion2D region)
        {
            SetTexCoords(region.Texture.Texel, region.Bounds);
        }

        public void SetTexCoords(Vector2 textureTexel, RectangleF sourceRect)
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
}