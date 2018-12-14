using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Glyph = MonoGame.Extended.BitmapFonts.BitmapFont.Glyph;

namespace MonoGame.Extended.BitmapFonts
{
    internal class GlyphEnumerator : IEnumerator<Glyph>
    {
        private bool _disposed;
        private BitmapFont _font;
        private ICharIterator _iterator;
        private Vector2 _position;

        private int _index;
        private Vector2 _positionDelta;

        private bool _hasPreviousGlyph;
        private Glyph _previousGlyph;
        public Glyph CurrentGlyph;
        public Glyph Current => CurrentGlyph;

        // casting a struct to object will box it, a behaviour we want to avoid...
        object IEnumerator.Current => throw new InvalidOperationException();

        public GlyphEnumerator(BitmapFont font, ICharIterator iterator, Vector2 position)
        {
            Set(font, iterator, position);
        }

        internal void Set(BitmapFont font, ICharIterator iterator, Vector2 position)
        {
            _disposed = false;
            _font = font;
            _iterator = iterator;
            _position = position;

            Reset();
        }

        public bool MoveNext()
        {
            _index++;
            if (_index >= _iterator.Length)
                return false;

            int character = _iterator.GetCharacter32(ref _index);
            BitmapFontRegion region = _font.GetCharacterRegion(character);
            Vector2 newPos = _position + _positionDelta;

            if (region != null)
            {
                newPos.X += region.XOffset;
                newPos.Y += region.YOffset;
                _positionDelta.X += region.XAdvance + _font.LetterSpacing;
            }

            if (_hasPreviousGlyph)
            {
                if (BitmapFont.UseKernings && _previousGlyph.FontRegion != null)
                    if (_previousGlyph.FontRegion.Kernings.TryGetValue(character, out int amount))
                        _positionDelta.X += amount;
            }

            CurrentGlyph = new Glyph(character, newPos, region);
            _previousGlyph = CurrentGlyph;
            _hasPreviousGlyph = true;

            if (character == '\n')
            {
                _positionDelta.Y += _font.LineHeight;
                _positionDelta.X = 0;
                _hasPreviousGlyph = false;
            }
            return true;
        }

        public void Reset()
        {
            _index = -1;
            _positionDelta = Vector2.Zero;
            _hasPreviousGlyph = false;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _font = null;
                _iterator.Dispose();
                _iterator = null;
                GlyphEnumeratorPool.Return(this);
            }
        }
    }
}
