using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public struct CharDrawPosition
    {
        public int Char;
        public Texture2D Texture;
        public Rectangle SourceRectangle;
        public Vector2 Position;
        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public float Depth;

        public CharDrawPosition(int character, Texture2D texture, Rectangle srcRect, Vector2 position,
            Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth)
        {
            Char = character;
            Texture = texture;
            SourceRectangle = srcRect;
            Position = position;
            Color = color;
            Rotation = rotation;
            Origin = origin;
            Scale = scale;
            Depth = layerDepth;
        }
    }
}
