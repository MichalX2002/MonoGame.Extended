using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace MonoGame.Extended.Content.Pipeline.Text
{
    [ContentProcessor(DisplayName = "Text Data Processor - MonoGame.Extended")]
    public class TextDataProcessor : ContentProcessor<TextDataFile, ProcessedTextData>
    {
        [DefaultValue(true)]
        public bool MinifyJson { get; set; } = true;

        [DefaultValue(true)]
        public bool MinifyXml { get; set; } = true;

        public override ProcessedTextData Process(
            TextDataFile input, ContentProcessorContext context)
        {
            string processed = null;
            switch (input.Type)
            {
                case TextDataType.Json:
                    if (MinifyJson)
                        processed = MinifyJsonSource(input.Data);
                    break;

                case TextDataType.Xaml:
                case TextDataType.Xml:
                    if (MinifyXml)
                        processed = MinifyXmlSource(input.Data);
                    break;
            }

            if(processed == null)
                using (var reader = new StreamReader(input.Data))
                    processed = reader.ReadToEnd();

            return new ProcessedTextData(processed);
        }

        private string MinifyJsonSource(Stream source)
        {
            using (var textReader = new StreamReader(source))
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
