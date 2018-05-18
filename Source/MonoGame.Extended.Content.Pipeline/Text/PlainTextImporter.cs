using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace MonoGame.Extended.Content.Pipeline.Text
{
    [ContentImporter(".txt", ".xml", ".json", ".lua", ".ini",
        DefaultProcessor = nameof(PlainTextProcessor),
        DisplayName = "Plain Text Importer - MonoGame.Extended"
    )]
    public class PlainTextImporter : ContentImporter<PlainTextFile>
    {
        public override PlainTextFile Import(string filename, ContentImporterContext context)
        {
            var fileInfo = new FileInfo(filename);
            var type = GetExtensionType(fileInfo.Extension);
            
            return new PlainTextFile(fileInfo.OpenRead(), type);
        }

        public PlainTextType GetExtensionType(string extension)
        {
            switch(extension.Replace(".", string.Empty).ToLower())
            {
                case "txt": return PlainTextType.Text;
                case "json": return PlainTextType.Json;
                case "xml": return PlainTextType.Xml;
                case "lua": return PlainTextType.Lua;

                default: return PlainTextType.Unknown;
            }
        }
    }
}
