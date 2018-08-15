
using System.IO;

namespace MonoGame.Extended.Content.Pipeline.Text
{
    public class TextDataFile
    {
        public FileStream Data { get; set; }
        public TextDataType Type { get; set; }

        public TextDataFile(FileStream data, TextDataType type)
        {
            Data = data;
            Type = type;
        }
    }
}
