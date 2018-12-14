using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// Provides additional methods for working with color
    /// </summary>
    public static class ColorExtensions
    {
        public static Color FromHex(string value)
        {
            var r = int.Parse(value.Substring(1, 2), NumberStyles.HexNumber);
            var g = int.Parse(value.Substring(3, 2), NumberStyles.HexNumber);
            var b = int.Parse(value.Substring(5, 2), NumberStyles.HexNumber);
            var a = value.Length > 7 ? int.Parse(value.Substring(7, 2), NumberStyles.HexNumber) : 255;
            return new Color(r, g, b, a);
        }

        public static HslColor ToHsl(this Color c)
        {
            float r = c.R / 255f;
            float b = c.B / 255f;
            float g = c.G / 255f;

            float max = Math.Max(Math.Max(r, g), b);
            float min = Math.Min(Math.Min(r, g), b);
            float chroma = max - min;
            float sum = max + min;

            float l = sum * 0.5f;

            if (chroma == 0)
                return new HslColor(0f, 0f, l);

            float h;
            if (r == max)
                h = (60 * (g - b) / chroma + 360) % 360;
            else
            {
                if (g == max)
                    h = 60 * (b - r) / chroma + 120f;
                else
                    h = 60 * (r - g) / chroma + 240f;
            }

            float s = l <= 0.5f ? chroma / sum : chroma / (2f - sum);

            return new HslColor(h, s, l);
        }
    }
}