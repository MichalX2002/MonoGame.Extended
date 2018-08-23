using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public struct GlyphBatchedSprite
    {
        public int Char;
        public int Index;
        public Texture2D Texture;
        public BatchedSprite Sprite;

        public GlyphBatchedSprite(int character, int index, Texture2D texture)
        {
            Char = character;
            Index = index;
            Texture = texture;
            Sprite = default;
        }

        public void SetColor(Color color)
        {
            Sprite.SetColor(color);
        }
    }
}
