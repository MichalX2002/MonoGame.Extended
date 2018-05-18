using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch batch, Texture2D texture,
            VertexPositionColorTexture vertexTL,
            VertexPositionColorTexture vertexTR,
            VertexPositionColorTexture vertexBL,
            VertexPositionColorTexture vertexBR,
            float? depth = null)
        {
            var item = batch.GetBatchItem();
            item.Texture = texture;
            item.SortKey = batch.GetSortKey(texture, depth ?? vertexTL.Position.Z);

            item.VertexTL = vertexTL;
            item.VertexTR = vertexTR;
            item.VertexBL = vertexBL;
            item.VertexBR = vertexBR;

            batch.FlushIfNeeded();
        }
    }
}
