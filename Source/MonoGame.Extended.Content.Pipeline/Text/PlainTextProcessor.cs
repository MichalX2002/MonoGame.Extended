using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace MonoGame.Extended.Content.Pipeline.Text
{
    [ContentProcessor(DisplayName = "Plain Text Processor - MonoGame.Extended")]
    public class PlainTextProcessor : ContentProcessor<PlainTextFile, ProcessedPlainText>
    {
        [DefaultValue(true)]
        public bool MinifyJson { get; set; } = true;

        [DefaultValue(true)]
        public bool MinifyXml { get; set; } = true;

        public override ProcessedPlainText Process(
            PlainTextFile input, ContentProcessorContext context)
        {
            string ReadTextFromInput()
            {
                using (var reader = new StreamReader(input.Data))
                    return reader.ReadToEnd();
            }

            string processed = null;

            switch (input.Type)
            {
                case PlainTextType.Json:
                    if (MinifyJson)
                        processed = MinifyJsonSource(ReadTextFromInput());
                    break;

                case PlainTextType.Xml:
                    if (MinifyXml)
                        processed = MinifyXmlSource(input.Data);
                    break;

                default:
                    processed = ReadTextFromInput();
                    break;
            }

            return new ProcessedPlainText(processed);
        }

        private string MinifyJsonSource(string source)
        {
            using (var textReader = new StringReader(source))
            using (var jsonReader = new JsonTextReader(textReader))
            using (var textWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.Formatting = Formatting.None;

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.Comment)
                        continue;

                    jsonWriter.WriteToken(jsonReader);
                }
                
                return textWriter.ToString();
            }
        }

        private string MinifyXmlSource(Stream source)
        {
            var xmlMinifier = new XmlMinifier(XmlMinifierSettings.Aggressive);
            return xmlMinifier.Minify(source);
        }
    }
}
