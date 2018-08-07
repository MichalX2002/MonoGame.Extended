using System;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontRegion
    {
        private IReadOnlyDictionary<int, int> _kernings;

        public int Character { get; }
        public TextureRegion2D TextureRegion { get; }
        public int XOffset { get; }
        public int YOffset { get; }
        public int XAdvance { get; }
        public int Width => TextureRegion.Width;
        public int Height => TextureRegion.Height;

        public IReadOnlyDictionary<int, int> Kernings
        {
            get
            {
                if (_kernings == null)
                    return EmptyDictionary<int, int>.ReadOnlyInstance;
                return _kernings;
            }
            internal set => _kernings = value;
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