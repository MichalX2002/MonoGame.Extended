using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class MatrixExtensions
    {
        public static bool Decompose(this Matrix matrix, out Vector2 position, out float rotation, out Vector2 scale)
        {

            if (matrix.Decompose(out Vector3 scale3, out Quaternion rotationQuaternion, out Vector3 position3))
            {
                var direction = Vector2.Transform(Vector2.UnitX, rotationQuaternion);
                rotation = (float)Math.Atan2(direction.Y, direction.X);
                position = new Vector2(position3.X, position3.Y);
                scale = new Vector2(scale3.X, scale3.Y);
                return true;
            }

            position = Vector2.Zero;
            rotation = 0;
            scale = Vector2.One;
            return false;
        }
    }
}