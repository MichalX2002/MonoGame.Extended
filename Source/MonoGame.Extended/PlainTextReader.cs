using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended
{
    public class PlainTextReader : ContentTypeReader<PlainText>
    {
        protected override PlainText Read(ContentReader input, PlainText existingInstance)
        {
            if (existingInstance != null)
                return existingInstance;

            return new PlainText(input.ReadString());
        }
    }
}