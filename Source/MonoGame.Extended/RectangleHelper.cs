using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGame.Extended
{
    public static class RectangleHelper
    {
        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a list of <see cref="PointF" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="result">The resulting rectangle.</param>
        public static void CreateFrom(IReadOnlyList<PointF> points, out RectangleF result)
        {
            PrimitivesHelper.CreateRectangleFromPoints(points, out PointF minimum, out PointF maximum);
            RectangleF.CreateFrom(minimum, maximum, out result);
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from a list of <see cref="PointF" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The resulting <see cref="RectangleF" />.</returns>
        public static RectangleF CreateFrom(IReadOnlyList<PointF> points)
        {
            CreateFrom(points, out RectangleF result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from the specified <see cref="RectangleF" /> transformed by
        ///     the specified <see cref="Matrix2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle to be transformed.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <param name="result">The resulting transformed rectangle.</param>
        /// <returns>
        ///     The <see cref="BoundingRectangle" /> from the <paramref name="rectangle" /> transformed by the
        ///     <paramref name="transformMatrix" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If a transformed <see cref="BoundingRectangle" /> is used for <paramref name="rectangle" /> then the
        ///         resulting <see cref="BoundingRectangle" /> will have the compounded transformation, which most likely is
        ///         not desired.
        ///     </para>
        /// </remarks>
        public static void Transform(
            RectangleF rectangle, Matrix2 transformMatrix, out RectangleF result)
        {
            var center = rectangle.Center;
            var halfExtents = (Vector2)rectangle.Size * 0.5f;

            PrimitivesHelper.TransformRectangle(ref center, ref halfExtents, transformMatrix);

            result.X = center.X - halfExtents.X;
            result.Y = center.Y - halfExtents.Y;
            result.Width = halfExtents.X * 2;
            result.Height = halfExtents.Y * 2;
        }

        /// <summary>
        ///     Computes the <see cref="RectangleF" /> from the specified <see cref="BoundingRectangle" /> transformed by
        ///     the
        ///     specified <see cref="Matrix2" />.
        /// </summary>
        /// <param name="rectangle">The bounding rectangle.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <returns>
        ///     The <see cref="BoundingRectangle" /> from the <paramref name="rectangle" /> transformed by the
        ///     <paramref name="transformMatrix" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If a transformed <see cref="BoundingRectangle" /> is used for <paramref name="rectangle" /> then the
        ///         resulting <see cref="BoundingRectangle" /> will have the compounded transformation, which most likely is
        ///         not desired.
        ///     </para>
        /// </remarks>
        public static RectangleF Transform(RectangleF rectangle, Matrix2 transformMatrix)
        {
            Transform(rectangle, transformMatrix, out RectangleF result);
            return result;
        }
    }
}
