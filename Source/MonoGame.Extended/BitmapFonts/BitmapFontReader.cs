using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
        {
            int textureAssetCount = reader.ReadInt32();
            Texture2D[] textures = new Texture2D[textureAssetCount];

            for (var i = 0; i < textureAssetCount; i++)
            {
                string assetName = reader.GetRelativeAssetName(reader.ReadString());
                textures[i] = reader.ContentManager.Load<Texture2D>(assetName);
            }

            int lineHeight = reader.ReadInt32();
            int regionCount = reader.ReadInt32();
            var characterMap = new Dictionary<int, BitmapFontRegion>(regionCount);

            for (int r = 0; r < regionCount; r++)
            {
                int character = reader.ReadInt32();
                int textureIndex = reader.ReadInt32();
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                int xOffset = reader.ReadInt32();
                int yOffset = reader.ReadInt32();
                int xAdvance = reader.ReadInt32();
                var textureRegion = new TextureRegion2D(textures[textureIndex], x, y, width, height);

                var bmpReg = new BitmapFontRegion(textureRegion, character, xOffset, yOffset, xAdvance);
                characterMap.Add(character, bmpReg);
            }
            
            int kerningsCount = reader.ReadInt32();
            for (int k = 0; k < kerningsCount; k++)
            {
                int first = reader.ReadInt32();
                int second = reader.ReadInt32();
                int amount = reader.ReadInt32();

                if (characterMap.TryGetValue(first, out BitmapFontRegion region))
                {
                    region.Kernings = new Dictionary<int, int>(1)
                    {
                        { second, amount }
                    };
                }
            }

            return new BitmapFont(reader.AssetName, characterMap, lineHeight);
        }
    }
}