using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public struct GlyphBatchedSprite
    {
        public int Char;
        public Texture2D Texture;
        public BatchedSprite Sprite;
        public int Index;

        public void SetColor(Color color)
        {
            Sprite.SetColor(color);
        }
    }
}
