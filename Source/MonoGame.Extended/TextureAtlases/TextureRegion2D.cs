using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureRegion2D
    {
        private Rectangle _bounds;

        public string Name { get; }
        public Texture2D Texture { get; protected set; }
        public object Tag { get; set; }

        public Rectangle Bounds { get => _bounds; protected set => SetBounds(value); }
        public int X => Bounds.X;
        public int Y => Bounds.Y;
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;
        public Size Size => new Size(Width, Height);

        public Vector2 Texel { get; private set; }
        public float TexelWidth => Texel.X;
        public float TexelHeight => Texel.Y;

        public TextureRegion2D(Texture2D texture, int x, int y, int width, int height)
            : this(null, texture, x, y, width, height)
        {
        }

        public TextureRegion2D(Texture2D texture, Rectangle region)
            : this(null, texture, region.X, region.Y, region.Width, region.Height)
        {
        }
        
        public TextureRegion2D(string name, Texture2D texture, Rectangle region)
            : this(name, texture, region.X, region.Y, region.Width, region.Height)
        {
        }

        public TextureRegion2D(Texture2D texture)
            : this(texture.Name, texture, 0, 0, texture.Width, texture.Height)
        {
        }

        public TextureRegion2D(string name, Texture2D texture, int x, int y, int width, int height)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Name = name;
            Bounds = new Rectangle(x, y, width, height);
        }

        private void SetBounds(Rectangle rectangle)
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