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
        public const float COLOR_DIVIDER = 1 / 255f;
        public const float ONE_DEGREE = 1 / 360f;
        public const float ONE_THIRD = 1f / 3f;
        public const float TWO_THIRD = 2f / 3f;

        public static Color FromHex(string value)
        {
            var r = int.Parse(value.Substring(1, 2), NumberStyles.HexNumber);
            var g = int.Parse(value.Substring(3, 2), NumberStyles.HexNumber);
            var b = int.Parse(value.Substring(5, 2), NumberStyles.HexNumber);
            var a = value.Length > 7 ? int.Parse(value.Substring(7, 2), NumberStyles.HexNumber) : 255;
            return new Color(r, g, b, a);
        }
        
        public static Color ToRgb(this HslColor c)
        {
            var s = c.S;
            var l = c.L;

            if (s == 0f)
                return new Color(l, l, l);

            var max = l < 0.5f ? l * (1 + s) : l + s - l * s;
            var min = 2f * l - max;

            var h = c.H * ONE_DEGREE;

            return new Color(
                ComponentFromHue(min, max, h + ONE_THIRD),
                ComponentFromHue(min, max, h), 
                ComponentFromHue(min, max, h - ONE_THIRD));
        }

        private static float ComponentFromHue(float m1, float m2, float h)
        {
            if (m1 < 0)
                return 0;

            if (h * 6f < 1f)
                return m1 + (m2 - m1) * 6f * h;
            if (h * 2f < 1f)
                return m2;
            if (h * 3f < 2f)
                return m1 + (m2 - m1) * (TWO_THIRD - h) * 6f;

            return m1;
        }

        public static HslColor ToHsl(this Color c)
        {
            float r = c.R * COLOR_DIVIDER;
            float b = c.B * COLOR_DIVIDER;
            float g = c.G * COLOR_DIVIDER;

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
