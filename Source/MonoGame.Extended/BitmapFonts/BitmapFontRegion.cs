using System;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontRegion
    {
        public int Character { get; }
        public TextureRegion2D TextureRegion { get; }
        public int XOffset { get; }
        public int YOffset { get; }
        public int XAdvance { get; }
        public int Width => TextureRegion.Width;
        public int Height => TextureRegion.Height;

        public IReadOnlyDictionary<int, int> Kernings { get; internal set; }

        public BitmapFontRegion(
            TextureRegion2D textureRegion, int character, int xOffset, int yOffset, int xAdvance)
        {
            TextureRegion = textureRegion;
            Character = character;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
            Kernings = EmptyDictionary<int, int>.Instance;
        }


        public override string ToString()
        {
            return $"{Convert.ToChar(Character)} {TextureRegion}";
        }
    }
}