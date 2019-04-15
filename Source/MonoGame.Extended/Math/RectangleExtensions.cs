using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class RectangleExtensions
    {
        /// <summary>
        ///     Computes the closest <see cref="PointF" /> on this <see cref="RectangleF" /> to a specified
        ///     <see cref="PointF" />.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>The closest <see cref="PointF" /> on this <see cref="RectangleF" /> to the <paramref name="point" />.</returns>
        public static PointF ClosestPointTo(this RectangleF rect, PointF point)
        {
            PrimitivesHelper.ClosestPointToPointFromRectangle(rect.TopLeft, rect.BottomRight, point, out PointF result);
            return result;
        }

        /// <summary>
        ///     Gets the corners of the rectangle in a clockwise direction starting at the top left.
        /// </summary>
        public static Point[] GetCorners(this Rectangle rectangle)
        {
            var corners = new Point[4];
            corners[0] = new Point(rectangle.Left, rectangle.Top);
            corners[1] = new Point(rectangle.Right, rectangle.Top);
            corners[2] = new Point(rectangle.Right, rectangle.Bottom);
            corners[3] = new Point(rectangle.Left, rectangle.Bottom);
            return corners;
        }

        /// <summary>
        ///     Gets the corners of the rectangle in a clockwise direction starting at the top left.
        /// </summary>
        public static Vector2[] GetCorners(this RectangleF rectangle)
        {
            var corners = new Vector2[4];
            corners[0] = new Vector2(rectangle.Left, rectangle.Top);
            corners[1] = new Vector2(rectangle.Right, rectangle.Top);
            corners[2] = new Vector2(rectangle.Right, rectangle.Bottom);
            corners[3] = new Vector2(rectangle.Left, rectangle.Bottom);
            return corners;
        }

        public static Rectangle Clip(this Rectangle rectangle, Rectangle clippingRectangle)
        {
            var clip = clippingRectangle;
            rectangle.X = clip.X > rectangle.X ? clip.X : rectangle.X;
            rectangle.Y = clip.Y > rectangle.Y ? clip.Y : rectangle.Y;
            rectangle.Width = rectangle.Right > clip.Right ? clip.Right - rectangle.X : rectangle.Width;
            rectangle.Height = rectangle.Bottom > clip.Bottom ? clip.Bottom - rectangle.Y : rectangle.Height;

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return Rectangle.Empty;

            return rectangle;
        }

        public static RectangleF Clip(this RectangleF rectangle, RectangleF clippingRectangle)
        {
            var clip = clippingRectangle;
            rectangle.X = clip.X > rectangle.X ? clip.X : rectangle.X;
            rectangle.Y = clip.Y > rectangle.Y ? clip.Y : rectangle.Y;
            rectangle.Width = rectangle.Right > clip.Right ? clip.Right - rectangle.X : rectangle.Width;
            rectangle.Height = rectangle.Bottom > clip.Bottom ? clip.Bottom - rectangle.Y : rectangle.Height;

            if(rectangle.Width <= 0 || rectangle.Height <= 0)
                return RectangleF.Empty;

            return rectangle;
        }
    }
}