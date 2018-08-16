using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MonoGame.Extended.Content.Pipeline.Text
{
    [ContentTypeWriter]
    public class TextDataWriter : ContentTypeWriter<ProcessedTextData>
    {
        protected override void Write(ContentWriter output, ProcessedTextData value)
        {
            output.Write((byte)value.Type);
            output.Write(value.Value);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.TextDataReader, MonoGame.Extended";
        }

        /*
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.TextData";
        }
        */

        /*
        public static PlainTextEncoding GetEncodingType(Encoding encoding)
        {
            switch (encoding.EncodingName)
            {
                case "Unicode (UTF-8)": return PlainTextEncoding.UTF8;
                case "Unicode": return PlainTextEncoding.UTF16;
            }

            throw new ArgumentException(
                $"Unknown {nameof(Encoding)}: {encoding.EncodingName}");
        }
        */
    }
}
