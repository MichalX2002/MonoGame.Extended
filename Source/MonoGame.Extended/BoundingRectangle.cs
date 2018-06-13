﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 77 

    /// <summary>
    ///     An axis-aligned, four sided, two dimensional box defined by a centre <see cref="Point2" /> and a radii
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         An <see cref="BoundingRectangle" /> is categorized by having its faces oriented in such a way that its
    ///         face normals are at all times parallel with the axes of the given coordinate system.
    ///     </para>
    ///     <para>
    ///         The <see cref="BoundingRectangle" /> of a rotated <see cref="BoundingRectangle" /> will be equivalent or larger
    ///         in size
    ///         than the original depending on the angle of rotation.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{T}" />
    [DebuggerDisplay("{" + nameof(DebugDisplayString) + ",nq}")]
    public struct BoundingRectangle : IEquatable<BoundingRectangle>, IEquatableByRef<BoundingRectangle>
    {
        /// <summary>
        ///     The <see cref="BoundingRectangle" /> with <see cref="Center" /> <see cref="Point2.Zero"/> and
        ///     <see cref="HalfExtents" /> set to <see cref="Vector2.Zero"/>.
        /// </summary>
        public static readonly BoundingRectangle Empty = new BoundingRectangle();

        /// <summary>
        ///     The centre position of this <see cref="BoundingRectangle" />.
        /// </summary>
        public Point2 Center;

        /// <summary>
        ///     The distance from the <see cref="Center" /> point along both axes to any point on the boundary of this
        ///     <see cref="BoundingRectangle" />.
        /// </summary>
        public Vector2 HalfExtents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoundingRectangle" /> structure from the specified centre
        ///     <see cref="Point2" /> and the radii <see cref="SizeF" />.
        /// </summary>
        /// <param name="center">The centre <see cref="Point2" />.</param>
        /// <param name="halfExtents">The radii <see cref="Vector2" />.</param>
        public BoundingRectangle(Point2 center, SizeF halfExtents)
        {
            Center = center;
            HalfExtents = halfExtents;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from a minimum <see cref="Point2" /> and maximum
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <param name="result">The resulting bounding rectangle.</param>
        public static void CreateFrom(in Point2 minimum, in Point2 maximum, out BoundingRectangle result)
        {
            result.Center = new Point2((maximum.X + minimum.X) * 0.5f, (maximum.Y + minimum.Y) * 0.5f);
            result.HalfExtents = new Vector2((maximum.X - minimum.X) * 0.5f, (maximum.Y - minimum.Y) * 0.5f);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from a minimum <see cref="Point2" /> and maximum
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <returns>The resulting <see cref="BoundingRectangle" />.</returns>
        public static BoundingRectangle CreateFrom(in Point2 minimum, in Point2 maximum)
        {
            CreateFrom(minimum, maximum, out BoundingRectangle result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from a list of <see cref="Point2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="result">The resulting bounding rectangle.</param>
        public static void CreateFrom(IReadOnlyList<Point2> points, out BoundingRectangle result)
        {
            PrimitivesHelper.CreateRectangleFromPoints(points, out Point2 minimum, out Point2 maximum);
            CreateFrom(minimum, maximum, out result);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from a list of <see cref="Point2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The resulting <see cref="BoundingRectangle" />.</returns>
        public static BoundingRectangle CreateFrom(IReadOnlyList<Point2> points)
        {
            CreateFrom(points, out BoundingRectangle result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from the specified <see cref="BoundingRectangle" /> transformed by
        ///     the
        ///     specified <see cref="Matrix2" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <param name="result">The resulting bounding rectangle.</param>
        /// <returns>
        ///     The <see cref="BoundingRectangle" /> from the <paramref name="boundingRectangle" /> transformed by the
        ///     <paramref name="transformMatrix" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If a transformed <see cref="BoundingRectangle" /> is used for <paramref name="boundingRectangle" /> then the
        ///         resulting <see cref="BoundingRectangle" /> will have the compounded transformation, which most likely is
        ///         not desired.
        ///     </para>
        /// </remarks>
        public static void Transform(ref BoundingRectangle boundingRectangle,
            in Matrix2 transformMatrix, out BoundingRectangle result)
        {
            PrimitivesHelper.TransformRectangle(ref boundingRectangle.Center, ref boundingRectangle.HalfExtents, transformMatrix);
            result.Center = boundingRectangle.Center;
            result.HalfExtents = boundingRectangle.HalfExtents;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that contains the two specified
        ///     <see cref="BoundingRectangle" /> structures.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <param name="result">The resulting bounding rectangle that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.</param>
        public static void Union(in BoundingRectangle first, in BoundingRectangle second, out BoundingRectangle result)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 6.5; Bounding Volume Hierarchies - Merging Bounding Volumes. pg 267

            var firstMinimum = first.Center - first.HalfExtents;
            var firstMaximum = first.Center + first.HalfExtents;
            var secondMinimum = second.Center - second.HalfExtents;
            var secondMaximum = second.Center + second.HalfExtents;

            var minimum = Point2.Minimum(firstMinimum, secondMinimum);
            var maximum = Point2.Maximum(firstMaximum, secondMaximum);

            result = CreateFrom(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that contains the two specified
        ///     <see cref="BoundingRectangle" /> structures.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     A <see cref="BoundingRectangle" /> that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.
        /// </returns>
        public static BoundingRectangle Union(in BoundingRectangle first, in BoundingRectangle second)
        {
            Union(first, second, out BoundingRectangle result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that contains both the specified
        ///     <see cref="BoundingRectangle" /> and this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     A <see cref="BoundingRectangle" /> that contains both the <paramref name="boundingRectangle" /> and
        ///     this
        ///     <see cref="BoundingRectangle" />.
        /// </returns>
        public BoundingRectangle Union(in BoundingRectangle boundingRectangle)
        {
            return Union(this, boundingRectangle);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that is in common between the two specified
        ///     <see cref="BoundingRectangle" /> structures.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <param name="result">The resulting bounding rectangle that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <see cref="Empty"/>.</param>
        public static void Intersection(in BoundingRectangle first, in BoundingRectangle second, out BoundingRectangle result)
        {
            var firstMinimum = first.Center - first.HalfExtents;
            var firstMaximum = first.Center + first.HalfExtents;
            var secondMinimum = second.Center - second.HalfExtents;
            var secondMaximum = second.Center + second.HalfExtents;

            var minimum = Point2.Maximum(firstMinimum, secondMinimum);
            var maximum = Point2.Minimum(firstMaximum, secondMaximum);

            if ((maximum.X < minimum.X) || (maximum.Y < minimum.Y))
                result = BoundingRectangle.Empty;
            else
                result = CreateFrom(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that is in common between the two specified
        ///     <see cref="BoundingRectangle" /> structures.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     A <see cref="BoundingRectangle" /> that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <see cref="Empty"/>.
        /// </returns>
        public static BoundingRectangle Intersection(in BoundingRectangle first, in BoundingRectangle second)
        {
            Intersection(first, second, out BoundingRectangle result);
            return result;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that is in common between the specified
        ///     <see cref="BoundingRectangle" /> and this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     A <see cref="BoundingRectangle" /> that is in common between both the <paramref name="boundingRectangle" /> and
        ///     this <see cref="BoundingRectangle"/>, if they intersect; otherwise, <see cref="Empty"/>.
        /// </returns>
        public BoundingRectangle Intersection(in BoundingRectangle boundingRectangle)
        {
            Intersection(this, boundingRectangle, out BoundingRectangle result);
            return result;
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="BoundingRectangle" /> structures intersect.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <paramref name="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(in BoundingRectangle first, in BoundingRectangle second)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 80

            var distance = first.Center - second.Center;
            var radii = first.HalfExtents + second.HalfExtents;
            return Math.Abs(distance.X) <= radii.X && Math.Abs(distance.Y) <= radii.Y;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BoundingRectangle" /> intersects with this
        ///     <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="boundingRectangle" /> intersects with this
        ///     <see cref="BoundingRectangle" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(in BoundingRectangle boundingRectangle)
        {
            return Intersects(this, boundingRectangle);
        }
        
        /// <summary>
        ///     Updates this <see cref="BoundingRectangle" /> from a list of <see cref="Point2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        public void UpdateFromPoints(IReadOnlyList<Point2> points)
        {
            var boundingRectangle = CreateFrom(points);
            Center = boundingRectangle.Center;
            HalfExtents = boundingRectangle.HalfExtents;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BoundingRectangle" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="boundingRectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(in BoundingRectangle boundingRectangle, in Point2 point)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 78

            var distance = boundingRectangle.Center - point;
            var radii = boundingRectangle.HalfExtents;

            return (Math.Abs(distance.X) <= radii.X) && (Math.Abs(distance.Y) <= radii.Y);
        }

        /// <summary>
        ///     Determines whether this <see cref="BoundingRectangle" /> contains the specified <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingRectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Contains(in Point2 point)
        {
            return Contains(this, point);
        }

        /// <summary>
        ///     Computes the squared distance from this <see cref="BoundingRectangle"/> to a <see cref="Point2"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The squared distance from this <see cref="BoundingRectangle"/> to the <paramref name="point"/>.</returns>
        public float SquaredDistanceTo(in Point2 point)
        {
            return PrimitivesHelper.SquaredDistanceToPointFromRectangle(Center - HalfExtents, Center + HalfExtents, point);
        }

        /// <summary>
        ///     Computes the closest <see cref="Point2" /> on this <see cref="BoundingRectangle" /> to a specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The closest <see cref="Point2" /> on this <see cref="BoundingRectangle" /> to the <paramref name="point" />.</returns>
        public Point2 ClosestPointTo(in Point2 point)
        {
            PrimitivesHelper.ClosestPointToPointFromRectangle(Center - HalfExtents, Center + HalfExtents, point, out Point2 result);
            return result;
        }

        /// <summary>
        ///     Compares two <see cref="BoundingRectangle" /> structures. The result specifies whether the values of the
        ///     <see cref="Center" /> and <see cref="HalfExtents" /> fields of the two <see cref="BoundingRectangle" /> structures
        ///     are equal.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Center" /> and <see cref="HalfExtents" /> fields of the two
        ///     <see cref="BoundingRectangle" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in BoundingRectangle first, in BoundingRectangle second)
        {
            return first.Equals(in second);
        }

        /// <summary>
        ///     Compares two <see cref="BoundingRectangle" /> structures. The result specifies whether the values of the
        ///     <see cref="Center" /> and <see cref="HalfExtents" /> fields of the two <see cref="BoundingRectangle" /> structures
        ///     are unequal.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Center" /> and <see cref="HalfExtents" /> fields of the two
        ///     <see cref="BoundingRectangle" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in BoundingRectangle first, in BoundingRectangle second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="BoundingRectangle" /> is equal to another
        ///     <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingRectangle" /> is equal to the <paramref name="boundingRectangle" />;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BoundingRectangle boundingRectangle)
        {
            return Equals(in boundingRectangle);
        }

        /// <summary>
        ///     Indicates whether this <see cref="BoundingRectangle" /> is equal to another <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingRectangle" /> is equal to the <paramref name="boundingRectangle" />;
        ///     otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(in BoundingRectangle boundingRectangle)
        {
            return (boundingRectangle.Center == Center) && (boundingRectangle.HalfExtents == HalfExtents);
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="BoundingRectangle" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="BoundingRectangle" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is BoundingRectangle bounds)
                return Equals(in bounds);
            return false;
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="BoundingRectangle" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="BoundingRectangle" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7 + Center.GetHashCode();
                return hash * 3 + HalfExtents.GetHashCode();
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Rectangle" /> to a <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="BoundingRectangle" />.
        /// </returns>
        public static implicit operator BoundingRectangle(in Rectangle rectangle)
        {
            var radii = new SizeF(rectangle.Width * 0.5f, rectangle.Height * 0.5f);
            var centre = new Point2(rectangle.X + radii.Width, rectangle.Y + radii.Height);
            return new BoundingRectangle(centre, radii);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="BoundingRectangle" /> to a <see cref="Rectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="Rectangle" />.
        /// </returns>
        public static implicit operator Rectangle(in BoundingRectangle boundingRectangle)
        {
            var minimum = boundingRectangle.Center - boundingRectangle.HalfExtents;
            return new Rectangle((int)minimum.X, (int)minimum.Y, (int)boundingRectangle.HalfExtents.X * 2,
                (int)boundingRectangle.HalfExtents.Y * 2);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="RectangleF" /> to a <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="BoundingRectangle" />.
        /// </returns>
        public static implicit operator BoundingRectangle(in RectangleF rectangle)
        {
            var radii = new SizeF(rectangle.Width * 0.5f, rectangle.Height * 0.5f);
            var centre = new Point2(rectangle.X + radii.Width, rectangle.Y + radii.Height);
            return new BoundingRectangle(centre, radii);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="BoundingRectangle" /> to a <see cref="RectangleF" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="Rectangle" />.
        /// </returns>
        public static implicit operator RectangleF(in BoundingRectangle boundingRectangle)
        {
            var minimum = boundingRectangle.Center - boundingRectangle.HalfExtents;
            return new RectangleF(minimum.X, minimum.Y, boundingRectangle.HalfExtents.X * 2,
                boundingRectangle.HalfExtents.Y * 2);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="BoundingRectangle" />.
        /// </returns>
        public override string ToString()
        {
            return $"Centre: {Center}, Radii: {HalfExtents}";
        }

        internal string DebugDisplayString => ToString();
    }
}