using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public struct GlyphSprite
    {
        public int Char;
        public Texture2D Texture;
        public int Index;
        public Rectangle SourceRect;
        public Vector2 Position;
        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public float Depth;
        public bool Visible;
    }
}
