using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace MonoGame.Extended.Content.Pipeline.Text
{
    [ContentImporter(
        TXT, XML, JSON, LUA, INI, JS, XAML, CSS, HTML,
        DefaultProcessor = nameof(TextDataProcessor),
        DisplayName = "Plain Text Importer - MonoGame.Extended"
    )]
    public class TextDataImporter : ContentImporter<TextDataFile>
    {
        public const string TXT = ".txt";
        public const string XML = ".xml";
        public const string JSON = ".json";
        public const string LUA = ".lua";
        public const string INI = ".ini";
        public const string JS = ".js";
        public const string XAML = ".xaml";
        public const string CSS = ".css";
        public const string HTML = ".html";

        public override TextDataFile Import(string filename, ContentImporterContext context)
        {
            var fileInfo = new FileInfo(filename);
            var type = GetExtensionType(fileInfo.Extension);
            
            return new TextDataFile(fileInfo.OpenRead(), type);
        }

        public TextDataType GetExtensionType(string extension)
        {
            switch(extension.ToLower())
            {
                case TXT: return TextDataType.Plain;
                case XML: return TextDataType.Xml;
                case JSON: return TextDataType.Json;
                case LUA: return TextDataType.Lua;
                case INI: return TextDataType.Ini;
                case JS: return TextDataType.JavaScript;
                case XAML: return TextDataType.Xaml;
                case CSS: return TextDataType.Css;
                case HTML: return TextDataType.Html;

                default: return TextDataType.Unknown;
            }
        }
    }
}
