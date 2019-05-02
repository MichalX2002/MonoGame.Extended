using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public partial class BitmapFont
    {
        private readonly IReadOnlyDictionary<int, BitmapFontRegion> _characterMap;

        public BitmapFont(string name, IEnumerable<BitmapFontRegion> regions, int lineHeight) :
            this(name, lineHeight)
        {
            int count = regions is ICollection collection ? collection.Count : 0;
            var temp = new Dictionary<int, BitmapFontRegion>(count);
            foreach (var region in regions)
                temp.Add(region.Character, region);
            _characterMap = temp;
        }

        public BitmapFont(
            string name, IReadOnlyDictionary<int, BitmapFontRegion> regions, int lineHeight) :
            this(name, lineHeight)
        {
            _characterMap = regions;
        }

        private BitmapFont(string name, int lineHeight)
        {
            Name = name;
            LineHeight = lineHeight;
        }

        public string Name { get; }
        public int LineHeight { get; }
        public int LetterSpacing { get; set; } = 0;
        public static bool UseKernings { get; set; } = true;

        public BitmapFontRegion GetCharacterRegion(int character)
        {
            return _characterMap.TryGetValue(character, out BitmapFontRegion region) ? region : null;
        }

        public bool GetCharacterRegion(int character, out BitmapFontRegion region)
        {
            return _characterMap.TryGetValue(character, out region);
        }

        public bool ContainsCharacterRegion(int character)
        {
            return _characterMap.ContainsKey(character);
        }

        public override string ToString()
        {
            return $"BitmapFont: {Name}";
        }

        public readonly struct Glyph
        {
            public readonly int Character;
            public readonly Vector2 Position;
            public readonly BitmapFontRegion FontRegion;

            public Glyph(int character, Vector2 position, BitmapFontRegion fontRegion)
            {
                Character = character;
                Position = position;
                FontRegion = fontRegion;
            }
        }
    }

    public struct BitmapFontGlyph
    {
        public int Character;
        public Vector2 Position;
        public BitmapFontRegion FontRegion;
    }
}