
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

            ShapeExtensions.DrawCircle(center, radius, color, thickness, _circlePointBuffer, _circleBuffer);

            Texture2D tex = BatchedSpriteExtensions.GetOnePixelTexture(batch);
            int pointsVisible = (int)(_circleBuffer.Length * progress);
            for (int i = 0; i < pointsVisible; i++)
                batch.DrawRef(tex, ref _circleBuffer[i]);
        }
    }
}