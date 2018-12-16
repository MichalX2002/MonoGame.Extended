using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;

namespace MonoGame.Extended.BitmapFonts
{
    public static class GlyphEnumeratorPool
    {
        public const int DefaultCapacity = 256;

        private static Bag<GlyphEnumerator> _enumerators;

        static GlyphEnumeratorPool()
        {
            _enumerators = new Bag<GlyphEnumerator>();
        }

        public static IEnumerator<BitmapFont.Glyph> Rent(BitmapFont font, ICharIterator iterator, Vector2 position)
        {
            lock (_enumerators)
            {
                if (_enumerators.TryTake(out var result))
                {
                    result.Set(font, iterator, position);
                    return result;
                }
            }
            return new GlyphEnumerator(font, iterator, position);
        }

        public static void Return(IEnumerator<BitmapFont.Glyph> enumerator)
        {
            if (enumerator is GlyphEnumerator glyphEnumerator)
            {
                lock (_enumerators)
                {
                    glyphEnumerator.Dispose();
                    if (_enumerators.Count < DefaultCapacity)
                        _enumerators.Add(glyphEnumerator);
                }
            }
            else
                throw new ArgumentException("The enumerator was not rented from this pool.");
        }
    }
}
