using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureRegion2D
    {
        private RectangleF _bounds;

        public string Name { get; }
        public Texture2D Texture { get; protected set; }
        public object Tag { get; set; }

        public RectangleF Bounds { get => _bounds; protected set => SetBounds(value); }
        public float X => Bounds.X;
        public float Y => Bounds.Y;
        public float Width => Bounds.Width;
        public float Height => Bounds.Height;
        public SizeF Size => _bounds.Size;

        public Vector2 Texel { get; private set; }
        public float TexelWidth => Texel.X;
        public float TexelHeight => Texel.Y;

        public TextureRegion2D(Texture2D texture, float x, float y, float width, float height)
            : this(null, texture, x, y, width, height)
        {
        }

        public TextureRegion2D(Texture2D texture, RectangleF region)
            : this(null, texture, region.X, region.Y, region.Width, region.Height)
        {
        }
        
        public TextureRegion2D(string name, Texture2D texture, RectangleF region)
            : this(name, texture, region.X, region.Y, region.Width, region.Height)
        {
        }

        public TextureRegion2D(Texture2D texture)
            : this(texture.Name, texture, 0, 0, texture.Width, texture.Height)
        {
        }

        public TextureRegion2D(string name, Texture2D texture, float x, float y, float width, float height)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Name = name;
            Bounds = new RectangleF(x, y, width, height);
        }

        private void SetBounds(RectangleF rectangle)
        {
            _bounds = rectangle;
            Texel = new Vector2(1f / _bounds.Width, 1f / _bounds.Height);
        }

        public override string ToString()
        {
            return $"{Name ?? string.Empty} {Bounds}";
        }
    }
}