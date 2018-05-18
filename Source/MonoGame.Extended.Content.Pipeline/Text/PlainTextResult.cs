
using System.IO;

namespace MonoGame.Extended.Content.Pipeline.Text
{
    public class PlainTextFile
    {
        public FileStream Data { get; set; }
        public PlainTextType Type { get; set; }

        public PlainTextFile(FileStream data, PlainTextType type)
        {
            Data = data;
            Type = type;
        }
    }
}
