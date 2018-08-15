
namespace MonoGame.Extended
{
    public class TextData
    {
        public TextDataType Type { get; }
        public string Value { get; }

        public TextData(TextDataType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        /*
        public static Encoding GetEncodingFrom(PlainTextEncoding encoding)
        {
            switch (encoding)
            {
                case PlainTextEncoding.UTF8: return Encoding.UTF8;
                case PlainTextEncoding.UTF16: return Encoding.Unicode;
            }

            throw new ArgumentException(
                $"Unknown {nameof(PlainTextEncoding)}: {encoding}");
        }
        */
    }
}
