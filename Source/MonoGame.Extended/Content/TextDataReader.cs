using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended
{
    public class TextDataReader : ContentTypeReader<TextData>
    {
        protected override TextData Read(ContentReader input, TextData existingInstance)
        {
            if (existingInstance != null)
                return existingInstance;

            TextDataType type = (TextDataType)input.ReadByte();
            string value = input.ReadString();
            return new TextData(type, value);
        }
    }
}