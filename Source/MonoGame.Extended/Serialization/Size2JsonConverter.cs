using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class Size2JsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sizeF = (SizeF) value;
            writer.WriteValue($"{sizeF.Width} {sizeF.Height}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var values = reader.ReadAsMultiDimensional<float>();

            if(values.Length == 2)
                return new SizeF(values[0], values[1]);

            if (values.Length == 1)
                return new SizeF(values[0], values[1]);

            throw new InvalidOperationException("Invalid size");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SizeF);
        }
    }
}