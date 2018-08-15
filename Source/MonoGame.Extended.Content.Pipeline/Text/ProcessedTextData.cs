namespace MonoGame.Extended.Content.Pipeline.Text
{
    public class ProcessedTextData
    {
        public TextDataType Type { get; }
        public string Value { get; }

        public ProcessedTextData(TextDataType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}