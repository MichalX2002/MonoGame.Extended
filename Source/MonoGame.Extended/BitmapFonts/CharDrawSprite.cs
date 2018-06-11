using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public struct CharDrawSprite
    {
        public int Char;
        public Texture2D Texture;
        public BatchedSprite Sprite;

        public void SetColor(in Color color)
        {
            Sprite.SetColor(color);
        }
    }
}
