using System.Collections;
using System.Collections.Generic;
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

            public void Dispose()
            {
            }

            public void Reset()
            {
                _positionDelta = new Point2();
                _index = -1;
                _previousGlyph = null;
            }
        }

        public struct StringBuilderGlyphEnumerable : IEnumerable<BitmapFontGlyph>
        {
            private readonly StringBuilderGlyphEnumerator _enumerator;

            public StringBuilderGlyphEnumerable(BitmapFont font, StringBuilder text, Point2? position)
            {
                _enumerator = new StringBuilderGlyphEnumerator(font, text, position);
            }

            public StringBuilderGlyphEnumerator GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator<BitmapFontGlyph> IEnumerable<BitmapFontGlyph>.GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _enumerator;
            }
        }

        public struct StringBuilderGlyphEnumerator : IEnumerator<BitmapFontGlyph>
        {
            private readonly BitmapFont _font;
            private readonly StringBuilder _text;
            private int _index;
            private readonly Point2 _position;
            private Vector2 _positionDelta;
            private BitmapFontGlyph _currentGlyph;
            private BitmapFontGlyph? _previousGlyph;

            object IEnumerator.Current
            {
                get
                {
                    // casting a struct to object will box it, behaviour we want to avoid...
                    throw new InvalidOperationException();
                }
            }

            public BitmapFontGlyph Current => _currentGlyph;

            public StringBuilderGlyphEnumerator(BitmapFont font, StringBuilder text, Point2? position)
            {
                _font = font;
                _text = text;
                _index = -1;
                _position = position ?? new Point2();
                _positionDelta = new Vector2();
                _currentGlyph = new BitmapFontGlyph();
                _previousGlyph = null;
            }

            public bool MoveNext()
            {
                if (++_index >= _text.Length)
                    return false;

                var character = GetUnicodeCodePoint(_text, ref _index);
                _currentGlyph = new BitmapFontGlyph
                {
                    Character = character,
                    FontRegion = _font.GetCharacterRegion(character),
                    Position = _position + _positionDelta
                };

                if (_currentGlyph.FontRegion != null)
                {
                    _currentGlyph.Position.X += _currentGlyph.FontRegion.XOffset;
                    _currentGlyph.Position.Y += _currentGlyph.FontRegion.YOffset;
                    _positionDelta.X += _currentGlyph.FontRegion.XAdvance + _font.LetterSpacing;
                }

                if (UseKernings && _previousGlyph.HasValue && _previousGlyph.Value.FontRegion != null)
                {
                    int amount;
                    if (_previousGlyph.Value.FontRegion.Kernings.TryGetValue(character, out amount))
                    { 
                        _positionDelta.X += amount;
                        _currentGlyph.Position.X += amount;
                    }
                }

                _previousGlyph = _currentGlyph;

                if (character != '\n')
                    return true;

                _positionDelta.Y += _font.LineHeight;
                _positionDelta.X = _position.X;
                _previousGlyph = null;

                return true;
            }

            private static int GetUnicodeCodePoint(StringBuilder text, ref int index)
            {
                return char.IsHighSurrogate(text[index]) && ++index < text.Length
                    ? char.ConvertToUtf32(text[index - 1], text[index])
                    : text[index];
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
                _positionDelta = new Point2();
                _index = -1;
                _previousGlyph = null;
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