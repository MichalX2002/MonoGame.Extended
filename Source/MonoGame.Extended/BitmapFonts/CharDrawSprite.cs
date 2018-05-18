using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public struct CharDrawSprite
    {
        public int Char;
        public Texture2D Texture;
        public BatchedSprite Sprite;

        public void SetColor(ref Color color)
        {
            ref BatchedSprite sprite = ref Sprite;
            sprite.SetColor(ref color);
        }
    }
}
