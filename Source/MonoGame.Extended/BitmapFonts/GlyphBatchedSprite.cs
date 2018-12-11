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
        
        public void SetColor(Color color)
        {
            Sprite.SetColor(ref color);
        }
    }
}
