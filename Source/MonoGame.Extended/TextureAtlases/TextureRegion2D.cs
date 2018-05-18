using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureRegion2D
    {
        public string Name { get; }
        public Texture2D Texture { get; protected set; }
        public object Tag { get; set; }

        public Rectangle Bounds { get; protected set; }
        public int X => Bounds.X;
        public int Y => Bounds.Y;
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;
        public Size2 Size => new Size2(Width, Height);

        public Vector2 Texel => Texture.Texel;
        public float TexelWidth => Texture.TexelWidth;
        public float TexelHeight => Texture.TexelHeight;

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

        public override string ToString()
        {
            return $"{Name ?? string.Empty} {Bounds}";
        }
    }
}