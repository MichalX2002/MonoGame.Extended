using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Testing
{
    public class CircularProgressBar
    {
        private Vector2[] _circlePointBuffer;
        private BatchedSprite[] _circleBuffer;

        public int Sides { get; }

        public CircularProgressBar(int sides)
        {
            Sides = sides;
        }

        public void Draw(SpriteBatch batch, Vector2 center, float radius, Color color, float thickness, float progress)
        {
            if (_circlePointBuffer == null)
            {
                _circlePointBuffer = new Vector2[Sides];
                _circleBuffer = new BatchedSprite[Sides];
            }

            ShapeDrawingExtensions.CreateCircle(radius, _circlePointBuffer);
            
            int pointsVisible = (int)(_circleBuffer.Length * progress);
            batch.DrawPolygon(center, _circlePointBuffer, pointsVisible, color, thickness);
        }
    }
}