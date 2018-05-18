
namespace MonoGame.Extended
{
    public class PlainText
    {
        public string Value { get; }

        public PlainText(string value)
        {
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
