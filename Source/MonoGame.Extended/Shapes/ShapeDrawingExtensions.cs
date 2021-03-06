﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended
{
    /// <summary>
    /// <see cref="SpriteBatch"/> extensions for drawing primitive shapes.
    /// </summary>
    public static class ShapeDrawingExtensions
    {
        private static readonly object _whitePixelLock = new object();
        private static TextureRegion2D _whitePixel;

        public static TextureRegion2D GetWhitePixelTexture(SpriteBatch spriteBatch)
        {
            lock (_whitePixelLock)
            {
                if (_whitePixel == null)
                {
                    var tex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Rgba32);
                    tex.SetData(new[] { Color.White }.AsSpan());

                    _whitePixel = new TextureRegion2D(tex, _rectOne);
                }
                return _whitePixel;
            }
        }

        public static void SetWhitePixelTexture(TextureRegion2D region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (region.Width != 1 || region.Height != 1)
                throw new ArgumentException("Both width and height must be 1.");

            _whitePixel = region;
        }
        
        private static readonly RectangleF _rectOne = new RectangleF(0, 0, 1, 1);

        /// <summary>
        ///     Draws a closed polygon from a <see cref="Polygon" /> shape
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// ///
        /// <param name="position">Where to position the polygon</param>
        /// <param name="polygon">The polygon to draw</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        public static void DrawPolygon(
            this SpriteBatch spriteBatch, Vector2 position, Polygon polygon, Color color, float thickness = 1f)
        {
            DrawPolygon(spriteBatch, position, polygon.Vertices, polygon.Vertices.Length, color, thickness);
        }

        /// <summary>
        ///     Draws a closed polygon from an array of points
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// ///
        /// <param name="offset">Where to offset the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="count">The amount of points to draw.</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        public static void DrawPolygon(
            this SpriteBatch spriteBatch, Vector2 offset, Vector2[] points, int count, Color color, float thickness = 1f)
        {
            if (count > points.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "Not enough elements in given array.");

            if (count == 0)
                return;

            if (count == 1)
            {
                DrawPoint(spriteBatch, points[0], color, (int)thickness);
                return;
            }

            var texture = GetWhitePixelTexture(spriteBatch);

            void DrawPolygonEdge(Vector2 point1, Vector2 point2)
            {
                var length = Vector2.Distance(point1, point2);
                var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
                var scale = new Vector2(length, thickness);
                spriteBatch.Draw(texture, point1, color, angle, Vector2.Zero, scale, 0, 0);
            }

            for (var i = 0; i < count - 1; i++)
                DrawPolygonEdge(points[i] + offset, points[i + 1] + offset);

            // connect the end with the start
            if (points.Length == count)
                DrawPolygonEdge(points[count - 1] + offset, points[0] + offset);
        }

        /// <summary>
        ///     Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rectangle">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        public static void DrawFilledRectangle(
            this SpriteBatch spriteBatch, RectangleF rectangle, Color color)
        {
            DrawFilledRectangle(spriteBatch, rectangle.Position, rectangle.Size, color);
        }

        /// <summary>
        ///     Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        public static void DrawFilledRectangle(
            this SpriteBatch spriteBatch, Vector2 location, SizeF size, Color color)
        {
            var texture = GetWhitePixelTexture(spriteBatch);
            spriteBatch.Draw(texture, location, color, 0, Vector2.Zero, size, SpriteEffects.None, 0);
        }

        /// <summary>
        ///     Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The X coord of the left side</param>
        /// <param name="y">The Y coord of the upper side</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="color">The color to draw the rectangle in</param>
        public static void DrawFilledRectangle(
            this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
        {
            DrawFilledRectangle(spriteBatch, new Vector2(x, y), new SizeF(width, height), color);
        }

        /// <summary>
        ///     Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rectangle">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the lines</param>
        public static void DrawRectangle(
            this SpriteBatch spriteBatch, RectangleF rectangle, Color color, float thickness = 1f)
        {
            var texture = GetWhitePixelTexture(spriteBatch);
            var topLeft = new Vector2(rectangle.X, rectangle.Y);
            var topRight = new Vector2(rectangle.Right - thickness, rectangle.Y);
            var bottomLeft = new Vector2(rectangle.X, rectangle.Bottom - thickness);
            var horizontalScale = new Vector2(rectangle.Width, thickness);
            var verticalScale = new Vector2(thickness, rectangle.Height);

            spriteBatch.Draw(texture, topLeft, color, 0, Vector2.Zero, horizontalScale, 0, 0);
            spriteBatch.Draw(texture, topLeft, color, 0, Vector2.Zero, verticalScale, 0, 0);
            spriteBatch.Draw(texture, topRight, color, 0, Vector2.Zero, verticalScale, 0, 0);
            spriteBatch.Draw(texture, bottomLeft, color, 0, Vector2.Zero, horizontalScale, 0, 0);
        }

        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawRectangle(
            this SpriteBatch spriteBatch, Vector2 location, SizeF size, Color color, float thickness = 1f)
        {
            DrawRectangle(spriteBatch, new RectangleF(location.X, location.Y, size.Width, size.Height), color, thickness);
        }


        /// <summary>
        /// Draws a rectangle outline.
        /// </summary>
        public static void DrawRectangle(
            this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, float thickness = 1f)
        {
            DrawRectangle(spriteBatch, new RectangleF(x, y, width, height), color, thickness);
        }

        /// <summary>
        ///     Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coord of the first point</param>
        /// <param name="y1">The Y coord of the first point</param>
        /// <param name="x2">The X coord of the second point</param>
        /// <param name="y2">The Y coord of the second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawLine(
            this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness = 1f)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
        }

        /// <summary>
        ///     Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawLine(
            this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            // calculate the distance between the two vectors
            var distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        /// <summary>
        ///     Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawLine(
            this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var texture = GetWhitePixelTexture(spriteBatch);
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        ///     Draws a point at the specified x, y position. The center of the point will be at the position.
        /// </summary>
        public static void DrawPoint(this SpriteBatch spriteBatch, float x, float y, Color color, float size = 1f)
        {
            DrawPoint(spriteBatch, new Vector2(x, y), color, size);
        }

        /// <summary>
        ///     Draws a point at the specified position. The center of the point will be at the position.
        /// </summary>
        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float size = 1f)
        {
            var texture = GetWhitePixelTexture(spriteBatch);
            var scale = new Vector2(size);
            var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
            spriteBatch.Draw(texture, position + offset, color, 0, Vector2.Zero, scale, 0, 0);
        }

        public static void DrawCircle(
            Vector2 center, float radius, Color color, float thickness, Vector2[] sides, BatchedSprite[] output)
        {
            DrawCircle(Vector2.One, _rectOne, center, radius, color, thickness, sides, output);
        }

        public static void DrawCircle(
            TextureRegion2D region, Vector2 center, float radius, Color color, float thickness, Vector2[] sides, BatchedSprite[] output)
        {
            DrawCircle(region.Texel, region.Bounds, center, radius, color, thickness, sides, output);
        }

        public static void DrawCircle(
            Vector2 textureTexel, RectangleF sourceRect, Vector2 center,
            float radius, Color color, float thickness, Vector2[] sides, BatchedSprite[] output)
        {
            if (output.Length < sides.Length)
                throw new ArgumentException("Array is too small.", nameof(output));

            void GetPolygonEdge(Vector2 point1, Vector2 point2, int index)
            {
                float length = Vector2.Distance(point1, point2);
                float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
                Vector2 scale = new Vector2(length, thickness);

                ref BatchedSprite sprite = ref output[index];
                sprite.SetTexCoords(textureTexel, sourceRect);
                sprite.SetTransform(Matrix2.CreateFrom(point1, -angle, scale), sourceRect.Size);
                sprite.SetColor(color);
            }

            CreateCircle(radius, sides);

            int len = sides.Length - 1;
            for (int i = 0; i < len; i++)
                GetPolygonEdge(sides[i] + center, sides[i + 1] + center, i);

            GetPolygonEdge(sides[sides.Length - 1] + center, sides[0] + center, len);
        }

        /// <summary>
        ///     Draw a circle from a <see cref="CircleF" /> shape
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="circle">The circle shape to draw</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        public static void DrawCircle(
            this SpriteBatch spriteBatch, CircleF circle, int sides, Color color, float thickness = 1f)
        {
            DrawCircle(spriteBatch, circle.Center, circle.Radius, sides, color, thickness);
        }

        /// <summary>
        /// Draw a circle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The center X of the circle</param>
        /// <param name="y">The center Y of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawCircle(
            this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness = 1f)
        {
            DrawCircle(spriteBatch, new Vector2(x, y), radius, sides, color, thickness);
        }

        /// <summary>
        /// Draw a circle.
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        public static void DrawCircle(
            this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1f)
        {
            var tmpSides = new Vector2[sides];
            CreateCircle(radius, tmpSides);
            DrawPolygon(spriteBatch, center, tmpSides, tmpSides.Length, color, thickness);
        }

        public static void CreateCircle(double radius, Vector2[] sides)
        {
            var step = MathHelper.TwoPi / sides.Length;
            var theta = -MathHelper.PiOver2;

            for (var i = 0; i < sides.Length; i++)
            {
                sides[i] = new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
                theta += step;
            }
        }
    }
}