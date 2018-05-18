using System;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontRegion
    {
        private static readonly Dictionary<int, int> _empty;

        private Dictionary<int, int> _kernings;

        public int Character { get; }
        public TextureRegion2D TextureRegion { get; }
        public int XOffset { get; }
        public int YOffset { get; }
        public int XAdvance { get; }
        public int Width => TextureRegion.Width;
        public int Height => TextureRegion.Height;

        public Dictionary<int, int> Kernings
        {
            get
            {
                if (_kernings == null)
                    return _empty;
                return _kernings;
            }
            internal set => _kernings = value;
        }

        static BitmapFontRegion()
        {
            _empty = new Dictionary<int, int>(0);
        }

        public BitmapFontRegion(
            TextureRegion2D textureRegion, int character, int xOffset, int yOffset, int xAdvance)
        {
            TextureRegion = textureRegion;
            Character = character;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
        }


        public override string ToString()
        {
            return $"{Convert.ToChar(Character)} {TextureRegion}";
        }
    }
}