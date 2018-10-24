using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace MonoGame.Extended
{
    // https://en.wikipedia.org/wiki/Matrix_(mathematics)
    // "Immersive Linear Algebra"; Jacob Ström, Kalle Åström & Tomas Akenine-Möller; 2015-2016. Chapter 6: The Matrix. http://immersivemath.com/ila/ch06_matrices/ch06.html
    // "Real-Time Collision Detection"; Christer Ericson; 2005. Chapter 3.1: A Math and Geometry Primer - Matrices. pg 23-34

    // Original code was from Matrix2D.cs in the Nez Library: https://github.com/prime31/Nez/

    /// <summary>
    ///     Defines a 3x3 matrix using floating point numbers which can store two dimensional translation, scale and rotation
    ///     information in a right-handed coordinate system.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Matrices use a row vector layout in the XNA / MonoGame Framework but, in general, matrices can be either have
    ///         a row vector or column vector layout. Row vector matrices view vectors as a row from left to right, while
    ///         column vector matrices view vectors as a column from top to bottom. For example, the <see cref="Translation" />
    ///         corresponds to the fields <see cref="M31" /> and <see cref="M32" />.
    ///     </para>
    ///     <para>
    ///         The fields M13 and M23 always have a value of <code>0.0f</code>, and thus are removed from the
    ///         <see cref="Matrix2" /> to reduce its memory footprint. Same is true for the field M33, except it always has a
    ///         value of <code>1.0f</code>.
    ///     </para>
    /// </remarks>
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    [DataContract]
    public struct Matrix2 : IEquatable<Matrix2>
    {
        [DataMember] public float M11; // x scale, also used for rotation
        [DataMember] public float M12; // used for rotation

        [DataMember] public float M21; // used for rotation
        [DataMember] public float M22; // y scale, also used for rotation

        [DataMember] public float M31; // x translation
        [DataMember] public float M32; // y translation

        /// <summary>
        ///     Gets the identity matrix.
        /// </summary>
        /// <value>
        ///     The identity matrix.
        /// </value>
        public static readonly Matrix2 Identity = new Matrix2(1f, 0f, 0f, 1f, 0f, 0f);

        /// <summary>
        ///     Gets the translation.
        /// </summary>
        /// <value>
        ///     The translation.
        /// </value>
        /// <remarks>The <see cref="Translation" /> is equal to the vector <code>(M31, M32)</code>.</remarks>
        public Vector2 Translation => new Vector2(M31, M32);

        /// <summary>
        ///     Gets the rotation angle in radians.
        /// </summary>
        /// <value>
        ///     The rotation angle in radians.
        /// </value>
        /// <remarks>
        ///     The <see cref="Rotation" /> is equal to <code>Atan2(M21, M11)</code>.
        /// </remarks>
        public float Rotation => (float)Math.Atan2(M21, M11);

        /// <summary>
        ///     Gets the scale.
        /// </summary>
        /// <value>
        ///     The scale.
        /// </value>
        /// <remarks>
        ///     The <see cref="Scale" /> is equal to the vector
        ///     <code>(Sqrt(M11 * M11 + M21 * M21), Sqrt(M12 * M12 + M22 * M22))</code>.
        /// </remarks>
        public Vector2 Scale
        {
            get
            {
                var scaleX = (float)Math.Sqrt(M11 * M11 + M21 * M21);
                var scaleY = (float)Math.Sqrt(M12 * M12 + M22 * M22);
                return new Vector2(scaleX, scaleY);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct.
        /// </summary>
        /// <param name="m11">The value to initialize <see cref="M11" /> to.</param>
        /// <param name="m12">The value to initialize <see cref="M12" /> to.</param>
        /// <param name="m21">The value to initialize <see cref="M21" /> to.</param>
        /// <param name="m22">The value to initialize <see cref="M22" /> to.</param>
        /// <param name="m31">The value to initialize <see cref="M31" /> to.</param>
        /// <param name="m32">The value to initialize <see cref="M32" /> to.</param>
        /// <remarks>
        ///     <para>
        ///         The fields M13 and M23 always have a value of <code>0.0f</code>, and thus are removed from the
        ///         <see cref="Matrix2" /> to reduce its memory footprint. Same is true for the field M33, except it always has a
        ///         value of <code>1.0f</code>.
        ///     </para>
        /// </remarks>
        public Matrix2(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            M11 = m11;
            M12 = m12;

            M21 = m21;
            M22 = m22;

            M31 = m31;
            M32 = m32;
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2" /> by this <see cref="Matrix2" />.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The resulting <see cref="Vector2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Transform(Vector2 vector)
        {
            Transform(vector, out Vector2 result);
            return result;
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2" /> by this <see cref="Matrix2" />.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="result">The resulting <see cref="Vector2" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Transform(Vector2 vector, out Vector2 result)
        {
            result.X = vector.X * M11 + vector.Y * M21 + M31;
            result.Y = vector.X * M12 + vector.Y * M22 + M32;
        }

        /// <summary>
        ///     Transforms <paramref name="x"/> and <paramref name="y"/> by this <see cref="Matrix2" />.
        /// </summary>
        /// <param name="x">The x value of the vector.</param>
        /// <param name="y">The y value of the vector.</param>
        /// <param name="result">The resulting <see cref="Vector2" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Transform(float x, float y, out Vector2 result)
        {
            result.X = x * M11 + y * M21 + M31;
            result.Y = x * M12 + y * M22 + M32;
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2" /> by this <see cref="Matrix2" />.
        /// </summary>
        /// <param name="x">The x value of the vector.</param>
        /// <param name="y">The y value of the vector.</param>
        /// <param name="result">The resulting <see cref="Vector2" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Transform(float x, float y, ref Vector3 result)
        {
            result.X = x * M11 + y * M21 + M31;
            result.Y = x * M12 + y * M22 + M32;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to translate, rotate, and scale a set of vertices in two dimensions.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <param name="rotation">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <param name="origin">The point which to rotate and scale around.</param>
        /// <param name="transformMatrix">The resulting <see cref="Matrix2" /></param>
        public static void CreateFrom(
            Vector2 position, float rotation, Vector2? scale, Vector2? origin, out Matrix2 transformMatrix)
        {
            transformMatrix = CreateFrom(position, rotation, scale, origin);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to translate, rotate, and scale a set of vertices in two dimensions.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <param name="rotation">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <param name="origin">The point which to rotate and scale around.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 CreateFrom(Vector2 position, float rotation, Vector2? scale = null, Vector2? origin = null)
        {
            Matrix2 transformMatrix = Identity;

            if (origin.HasValue)
            {
                transformMatrix.M31 = -origin.Value.X;
                transformMatrix.M32 = -origin.Value.Y;
            }

            if (scale.HasValue)
            {
                transformMatrix = Multiply(transformMatrix, CreateScale(scale.Value));
            }
            
            if (rotation != 0f)
            {
                transformMatrix = Multiply(transformMatrix, CreateRotationZ(-rotation));
            }

            return Multiply(transformMatrix, CreateTranslation(position));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to rotate a set of vertices
        ///     around the z-axis.
        /// </summary>
        /// <param name="radians">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 CreateRotationZ(float radians)
        {
            float val1 = (float)Math.Cos(radians);
            float val2 = (float)Math.Sin(radians);

            return new Matrix2(val1, val2, -val2, val1, 0, 0);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> struct that can be used to rotate a set of vertices around the z-axis.
        /// </summary>
        /// <param name="radians">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void CreateRotationZ(float radians, out Matrix2 result)
        {
            result = CreateRotationZ(radians);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 CreateScale(float scale)
        {
            return CreateScale(scale, scale);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void CreateScale(float scale, out Matrix2 result)
        {
            CreateScale(scale, scale, out result);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="xScale">The amount to scale by on the x-axis.</param>
        /// <param name="yScale">The amount to scale by on the y-axis.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 CreateScale(float xScale, float yScale)
        {
            return new Matrix2(xScale, 0, 0, yScale, 0, 0);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="xScale">The amount to scale by on the x-axis.</param>
        /// <param name="yScale">The amount to scale by on the y-axis.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void CreateScale(float xScale, float yScale, out Matrix2 result)
        {
            result = CreateScale(xScale, yScale);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amounts to scale by on the x and y axes.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 CreateScale(Vector2 scale)
        {
            return new Matrix2(scale.X, 0, 0, scale.Y, 0, 0);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amounts to scale by on the x and y axes.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void CreateScale(Vector2 scale, out Matrix2 result)
        {
            result = CreateScale(scale);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="xPosition">The amount to translate by on the x-axis.</param>
        /// <param name="yPosition">The amount to translate by on the y-axis.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 CreateTranslation(float xPosition, float yPosition)
        {
            return new Matrix2(
                1, 0,
                0, 1,
                xPosition, yPosition);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="xPosition">The amount to translate by on the x-axis.</param>
        /// <param name="yPosition">The amount to translate by on the y-axis.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void CreateTranslation(float xPosition, float yPosition, out Matrix2 result)
        {
            result = CreateTranslation(xPosition, yPosition);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 CreateTranslation(Vector2 position)
        {
            return new Matrix2(
                1, 0,
                0, 1,
                position.X, position.Y);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void CreateTranslation(Vector2 position, out Matrix2 result)
        {
            result = CreateTranslation(position);
        }

        /// <summary>
        ///     Calculates the determinant of the <see cref="Matrix2" />.
        /// </summary>
        /// <returns>The determinant of the <see cref="Matrix2" />.</returns>
        public float Determinant()
        {
            return M11 * M22 - M12 * M21;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the summation of two <see cref="Matrix2" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 Add(Matrix2 matrix1, Matrix2 matrix2)
        {
            return new Matrix2(
                matrix1.M11 + matrix2.M11,
                matrix1.M12 + matrix2.M12,
                matrix1.M21 + matrix2.M21,
                matrix1.M22 + matrix2.M22,
                matrix1.M31 + matrix2.M31,
                matrix1.M32 + matrix2.M32);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> summation of two <see cref="Matrix2" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void Add(Matrix2 matrix1, Matrix2 matrix2, out Matrix2 result)
        {
            result = Add(matrix1, matrix2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the summation of two <see cref="Matrix2" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 operator +(Matrix2 matrix1, Matrix2 matrix2)
        {
            return Add(matrix1, matrix2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the substraction of two
        ///     <see cref="Matrix2" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 Subtract(Matrix2 matrix1, Matrix2 matrix2)
        {
            return new Matrix2(
                matrix1.M11 - matrix2.M11,
                matrix1.M12 - matrix2.M12,
                matrix1.M21 - matrix2.M21,
                matrix1.M22 - matrix2.M22,
                matrix1.M31 - matrix2.M31,
                matrix1.M32 - matrix2.M32);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> substraction of two <see cref="Matrix2" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void Subtract(Matrix2 matrix1, Matrix2 matrix2, out Matrix2 result)
        {
            result = Subtract(matrix1, matrix2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the substraction of two
        ///     <see cref="Matrix2" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 operator -(Matrix2 matrix1, Matrix2 matrix2)
        {
            return Subtract(matrix1, matrix2);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> multiplication of two <see cref="Matrix2" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void Multiply(Matrix2 matrix1, Matrix2 matrix2, out Matrix2 result)
        {
            result = Multiply(matrix1, matrix2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the multiplication of two
        ///     <see cref="Matrix2" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 Multiply(Matrix2 matrix1, Matrix2 matrix2)
        {
            return new Matrix2(
                matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21,
                matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22,
                matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21,
                matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22,
                matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix2.M31,
                matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix2.M32);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the division of a <see cref="Matrix2" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 Multiply(Matrix2 matrix, float scalar)
        {
            return new Matrix2(
                matrix.M11 * scalar,
                matrix.M12 * scalar,
                matrix.M21 * scalar,
                matrix.M22 * scalar,
                matrix.M31 * scalar,
                matrix.M32 * scalar);
        }

        /// <summary>
        ///     Calculates the multiplication of a <see cref="Matrix2" /> by a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="scalar">The amount to multiple the <see cref="Matrix2" /> by.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void Multiply(Matrix2 matrix, float scalar, out Matrix2 result)
        {
            result = Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the multiplication of two
        ///     <see cref="Matrix2" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 operator *(Matrix2 matrix1, Matrix2 matrix2)
        {
            return Multiply(matrix1, matrix2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the division of a <see cref="Matrix2" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 operator *(Matrix2 matrix, float scalar)
        {
            return Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the division of two <see cref="Matrix2" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 Divide(Matrix2 matrix1, Matrix2 matrix2)
        {
            return new Matrix2(
                matrix1.M11 / matrix2.M11,
                matrix1.M12 / matrix2.M12,
                matrix1.M21 / matrix2.M21,
                matrix1.M22 / matrix2.M22,
                matrix1.M31 / matrix2.M31,
                matrix1.M32 / matrix2.M32);
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2" /> division of two <see cref="Matrix2" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void Divide(Matrix2 matrix1, Matrix2 matrix2, out Matrix2 result)
        {
            result = Divide(matrix1, matrix2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the division of a <see cref="Matrix2" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 Divide(Matrix2 matrix, float scalar)
        {
            float num = 1f / scalar;
            return new Matrix2(
                matrix.M11 * num,
                matrix.M12 * num,
                matrix.M21 * num,
                matrix.M22 * num,
                matrix.M31 * num,
                matrix.M32 * num);
        }

        /// <summary>
        ///     Calculates the division of a <see cref="Matrix2" /> by a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2" /> by.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void Divide(Matrix2 matrix, float scalar, out Matrix2 result)
        {
            result = Divide(matrix, scalar);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the division of two <see cref="Matrix2" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 operator /(Matrix2 matrix1, Matrix2 matrix2)
        {
            return Divide(matrix1, matrix2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the division of a <see cref="Matrix2" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 operator /(Matrix2 matrix, float scalar)
        {
            return Divide(matrix, scalar);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the inversion of a <see cref="Matrix2" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 Invert(Matrix2 matrix)
        {
            float det = 1 / matrix.Determinant();
            return new Matrix2(
                matrix.M22 * det,
                -matrix.M12 * det,
                -matrix.M21 * det,
                matrix.M11 * det,
                (matrix.M32 * matrix.M21 - matrix.M31 * matrix.M22) * det,
                -(matrix.M32 * matrix.M11 - matrix.M31 * matrix.M12) * det);
        }

        /// <summary>
        ///     Calculates the inversion of a <see cref="Matrix2" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2" />.</param>
        public static void Invert(Matrix2 matrix, out Matrix2 result)
        {
            result = Invert(matrix);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2" /> struct with the inversion of a <see cref="Matrix2" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <returns>The resulting <see cref="Matrix2" />.</returns>
        public static Matrix2 operator -(Matrix2 matrix)
        {
            return new Matrix2(
                -matrix.M11,
                -matrix.M12,
                -matrix.M21,
                -matrix.M22,
                -matrix.M31,
                -matrix.M32);
        }

        /// <summary>
        ///     Compares a <see cref="Matrix2" /> for equality with another <see cref="Matrix2" /> without any tolerance.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns><c>true</c> if the <see cref="Matrix2" />s are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Matrix2 matrix1, Matrix2 matrix2)
        {
            return (matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12) && (matrix1.M21 == matrix2.M21) &&
                   (matrix1.M22 == matrix2.M22) && (matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32);
        }

        /// <summary>
        ///     Compares a <see cref="Matrix2" /> for inequality with another <see cref="Matrix2" /> without any tolerance.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2" />.</param>
        /// <returns><c>true</c> if the <see cref="Matrix2" />s are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Matrix2 matrix1, Matrix2 matrix2)
        {
            return (matrix1.M11 != matrix2.M11) || (matrix1.M12 != matrix2.M12) || (matrix1.M21 != matrix2.M21) ||
                   (matrix1.M22 != matrix2.M22) || (matrix1.M31 != matrix2.M31) || (matrix1.M32 != matrix2.M32);
        }

        /// <summary>
        ///     Returns a value that indicates whether the current <see cref="Matrix2" /> is equal to a specified
        ///     <see cref="Matrix2" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" /> with which to make the comparison.</param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="Matrix2" /> is equal to the specified <see cref="Matrix2" />;
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool Equals(Matrix2 matrix)
        {
            return (M11 == matrix.M11) && (M12 == matrix.M12) && (M21 == matrix.M21) && (M22 == matrix.M22) &&
                   (M31 == matrix.M31) && (M32 == matrix.M32);
        }

        /// <summary>
        ///     Returns a value that indicates whether the current <see cref="Matrix2" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object with which to make the comparison.</param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="Matrix2" /> is equal to the specified object;
        ///     <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix2 matrix)
                return Equals(matrix);
            return false;
        }

        /// <summary>
        ///     Returns a hash code for this <see cref="Matrix2" />.
        /// </summary>
        /// <returns>
        ///     A hash code for this <see cref="Matrix2" />, suitable for use in hashing algorithms and data structures like a
        ///     hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3 + M11.GetHashCode();
                hash = hash * 7 + M12.GetHashCode();
                hash = hash * 7 + M21.GetHashCode();
                hash = hash * 7 + M22.GetHashCode();
                hash = hash * 7 + M31.GetHashCode();
                hash = hash * 7 + M32.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Matrix2" /> to <see cref="Matrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <returns>
        ///     The resulting <see cref="Matrix" />.
        /// </returns>
        public static implicit operator Matrix(Matrix2 matrix)
        {
            return new Matrix(
                matrix.M11, matrix.M12, 0, 0,
                matrix.M21, matrix.M22, 0, 0,
                0, 0, 1, 0,
                matrix.M31, matrix.M32, 0, 1);
        }

        /// <summary>
        ///     Performs an explicit conversion from a specified <see cref="Matrix2" /> to a <see cref="Matrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2" />.</param>
        /// <param name="depth">The depth value.</param>
        /// <param name="result">The resulting <see cref="Matrix" />.</param>
        public static void ToMatrix(Matrix2 matrix, float depth, out Matrix result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M13 = 0;
            result.M14 = 0;

            result.M21 = matrix.M21;
            result.M22 = matrix.M22;
            result.M23 = 0;
            result.M24 = 0;

            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1;
            result.M34 = 0;

            result.M41 = matrix.M31;
            result.M42 = matrix.M32;
            result.M43 = depth;
            result.M44 = 1;
        }

        /// <summary>
        ///     Performs an explicit conversion from a specified <see cref="Matrix2" /> to a <see cref="Matrix" />.
        /// </summary>
        /// <param name="depth">The depth value.</param>
        /// <returns>The resulting <see cref="Matrix" />.</returns>
        public Matrix ToMatrix(float depth = 0)
        {
            ToMatrix(this, depth, out Matrix result);
            return result;
        }

        /// <summary>
        ///     Gets the debug display string.
        /// </summary>
        /// <value>
        ///     The debug display string.
        /// </value>
        internal string DebugDisplayString => this == Identity
                ? "Identity"
                : $"T:({Translation.X:0.##},{Translation.Y:0.##}), R:{MathHelper.ToDegrees(Rotation):0.##}°, S:({Scale.X:0.##},{Scale.Y:0.##})"
            ;

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Matrix2" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="Matrix2" />.
        /// </returns>
        public override string ToString()
        {
            return $"{{M11:{M11} M12:{M12}}} {{M21:{M21} M22:{M22}}} {{M31:{M31} M32:{M32}}}";
        }
    }
}