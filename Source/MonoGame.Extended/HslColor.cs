using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// A data structure representing a 24bit color composed of separate hue, saturation and lightness channels.
    /// </summary>
    //[Serializable]
    [DataContract]
    public struct HslColor : IEquatable<HslColor>, IComparable<HslColor>
    {
        internal float _h;
        internal float _s;
        internal float _l;

        /// <summary>
        /// Gets the value of the hue channel in degrees.
        /// </summary>
        [DataMember] public float H { get => _h; set => _h = NormalizeHue(value); }

        /// <summary>
        /// Gets the value of the saturation channel.
        /// </summary>
        [DataMember] public float S { get => _s; set => _s = MathHelper.Clamp(value, 0f, 1f); }

        /// <summary>
        /// Gets the value of the lightness channel.
        /// </summary>
        [DataMember] public float L { get => _l; set => _l = MathHelper.Clamp(value, 0f, 1f); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NormalizeHue(float h)
        {
            if (h < 0)
                return h + 360 * ((int)(h / 360) + 1);
            return h % 360;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HslColor" /> structure.
        /// </summary>
        /// <param name="h">The value of the hue channel.</param>
        /// <param name="s">The value of the saturation channel.</param>
        /// <param name="l">The value of the lightness channel.</param>
        public HslColor(float h, float s, float l)
        {
            // normalize the hue
            _h = NormalizeHue(h);
            _s = MathHelper.Clamp(s, 0f, 1f);
            _l = MathHelper.Clamp(l, 0f, 1f);
        }

        public HslColor(Color color)
        {
            // derived from http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            _h = 0f; // default to black
            _s = 0f;
            _l = 0f;
            float v = Math.Max(r, g);
            v = Math.Max(v, b);

            float m = Math.Min(r, g);
            m = Math.Min(m, b);
            _l = (m + v) / 2.0f;

            if (_l <= 0.0)
                return;

            var vm = v - m;
            _s = vm;

            if (_s > 0.0)
                _s /= _l <= 0.5f ? v + m : 2.0f - v - m;
            else
                return;

            float r2 = (v - r) / vm;
            float g2 = (v - g) / vm;
            float b2 = (v - b) / vm;

            if (Math.Abs(r - v) < float.Epsilon)
                _h = Math.Abs(g - m) < float.Epsilon ? 5.0f + b2 : 1.0f - g2;
            else if (Math.Abs(g - v) < float.Epsilon)
                _h = Math.Abs(b - m) < float.Epsilon ? 1.0f + r2 : 3.0f - b2;
            else
                _h = Math.Abs(r - m) < float.Epsilon ? 3.0f + g2 : 5.0f - r2;

            _h = NormalizeHue(_h * 60);
        }

        /// <summary>
        /// Copies the individual channels of the color to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the axis to.</param>
        public void CopyTo(out HslColor destination)
        {
            destination = new HslColor(_h, _s, _l);
        }

        /// <summary>
        /// Destructures the color, exposing the individual channels.
        /// </summary>
        public void Destructure(out float h, out float s, out float l)
        {
            h = _h;
            s = _s;
            l = _l;
        }

        /// <summary>
        /// Exposes the individual channels of the color to the specified matching function.
        /// </summary>
        /// <param name="callback">The function which matches the individual channels of the color.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="callback" /> parameter is <c>null</c>.
        /// </exception>
        public void Match(Action<float, float, float> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_h, _s, _l);
        }

        /// <summary>
        /// Exposes the individual channels of the color to the
        /// specified mapping function and returns the result.
        /// </summary>
        /// <typeparam name="T">The type being mapped to.</typeparam>
        /// <param name="map">
        /// A function which maps the color channels to an instance of <typeparamref name="T" />.
        /// </param>
        /// <returns>
        /// The result of the <paramref name="map"/> function when passed the individual X and Y components.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="map" /> parameter is <c>null</c>.
        /// </exception>
        public T Map<T>(Func<float, float, float, T> map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(_h, _s, _l);
        }

        public static HslColor operator +(HslColor a, HslColor b)
        {
            return new HslColor(a._h + b._h, a._s + b._s, a._l + b._l);
        }

        public static implicit operator HslColor(string value)
        {
            return Parse(value);
        }

        public int CompareTo(HslColor other)
        {
            // ReSharper disable ImpureMethodCallOnReadonlyValueField
            return _h.CompareTo(other._h) * 100 + _s.CompareTo(other._s) * 10 + _l.CompareTo(_l);
            // ReSharper restore ImpureMethodCallOnReadonlyValueField
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is HslColor)
                return Equals((HslColor)obj);

            return base.Equals(obj);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="HslColor" /> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="HslColor" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="HslColor" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(HslColor value)
        {
            // ReSharper disable ImpureMethodCallOnReadonlyValueField
            return _h.Equals(value._h) && _s.Equals(value._s) && _l.Equals(value._l);
            // ReSharper restore ImpureMethodCallOnReadonlyValueField
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _h.GetHashCode() ^
                   _s.GetHashCode() ^
                   _l.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, "H:{0:N1}° S:{1:N1} L:{2:N1}", _h, 100 * _s, 100 * _l);
        }

        public static HslColor Parse(string s)
        {
            string[] hsl = s.Split(',');
            float hue = float.Parse(hsl[0].TrimEnd('°'), CultureInfo.InvariantCulture.NumberFormat);
            float sat = float.Parse(hsl[1], CultureInfo.InvariantCulture.NumberFormat);
            float lig = float.Parse(hsl[2], CultureInfo.InvariantCulture.NumberFormat);

            return new HslColor(hue, sat, lig);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="HslColor" /> is equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(HslColor x, HslColor y)
        {
            return x.Equals(y);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="HslColor" /> is not equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(HslColor x, HslColor y)
        {
            return !x.Equals(y);
        }

        public static HslColor operator -(HslColor a, HslColor b)
        {
            return new HslColor(a._h - b._h, a._s - b._s, a._l - b._l);
        }

        public static HslColor Lerp(HslColor c1, HslColor c2, float t)
        {
            // loop around if c2.H < c1.H
            float h2 = c2._h >= c1._h ? c2._h : c2._h + 360;
            return new HslColor(
                c1._h + t * (h2 - c1._h),
                c1._s + t * (c2._s - c1._s),
                c1._l + t * (c2._l - c2._l));
        }

        public Color ToRgb()
        {
            if (_s == 0f)
                return new Color(_l, _l, _l);

            float h = _h / 360f;
            var max = _l < 0.5f ? _l * (1 + _s) : _l + _s - _l * _s;
            var min = 2f * _l - max;

            return new Color(
                ComponentFromHue(min, max, h + 1f / 3f),
                ComponentFromHue(min, max, h),
                ComponentFromHue(min, max, h - 1f / 3f));
        }
        
        private static float ComponentFromHue(float m1, float m2, float h)
        {
            h -= (int)h; // h % 1f

            if (h * 6f < 1)
                return m1 + (m2 - m1) * 6f * h;
            if (h * 2 < 1)
                return m2;
            if (h * 3 < 2)
                return m1 + (m2 - m1) * (2f / 3f - h) * 6f;
            return m1;
        }
    }
}