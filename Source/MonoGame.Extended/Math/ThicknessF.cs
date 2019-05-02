using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace MonoGame.Extended
{
    [DataContract]
    public struct ThicknessF : IEquatable<ThicknessF>
    {
        [DataMember] public float Left { get; set; }
        [DataMember] public float Top { get; set; }
        [DataMember] public float Right { get; set; }
        [DataMember] public float Bottom { get; set; }

        public float Width => Left + Right;
        public float Height => Top + Bottom;
        public SizeF Size => new SizeF(Width, Height);

        public ThicknessF(float all)
            : this(all, all, all, all)
        {
        }

        public ThicknessF(float leftRight, float topBottom)
            : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public ThicknessF(float left, float top, float right, float bottom)
            : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static implicit operator ThicknessF(float value)
        {
            return new ThicknessF(value);
        }

        public override bool Equals(object obj)
        {
            if (obj is ThicknessF other)
                return Equals(other);

            return base.Equals(obj);
        }

        public bool Equals(ThicknessF other)
        {
            return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Left.GetHashCode();
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                hashCode = (hashCode * 397) ^ Right.GetHashCode();
                hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
                return hashCode;
            }
        }

        public static ThicknessF FromValues(float[] values)
        {
            switch (values.Length)
            {
                case 1:
                    return new ThicknessF(values[0]);
                case 2:
                    return new ThicknessF(values[0], values[1]);
                case 4:
                    return new ThicknessF(values[0], values[1], values[2], values[3]);
                default:
                    throw new FormatException("Invalid thickness");
            }
        }

        public static ThicknessF Parse(string value)
        {
            float[] values = value
                .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(float.Parse)
                .ToArray();

            return FromValues(values);
        }

        public override string ToString()
        {
            if (Left == Right && Top == Bottom)
                return Left == Top ? $"{Left}" : $"{Left} {Top}";

            return $"{Left}, {Right}, {Top}, {Bottom}";
        }
    }
}