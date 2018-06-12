using System;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class TextureRegion2DJsonConverter : JsonConverter
    {
        private readonly ITextureRegionService _textureRegionService;

        public TextureRegion2DJsonConverter(ITextureRegionService textureRegionService)
        {
            _textureRegionService = textureRegionService;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var region = (TextureRegion2D)value;
            writer.WriteValue(region.Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return !(reader.Value is string regionName) ? null : _textureRegionService.GetTextureRegion(regionName);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TextureRegion2D);
        }
    }
}