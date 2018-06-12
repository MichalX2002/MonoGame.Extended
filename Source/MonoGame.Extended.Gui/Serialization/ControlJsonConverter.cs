using System;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class ControlJsonConverter : JsonConverter
    {
        private readonly IGuiSkinService _guiSkinService;
        private readonly ControlStyleJsonConverter _styleConverter;
        private const string _styleProperty = "Style";

        public ControlJsonConverter(IGuiSkinService guiSkinService, params Type[] customControlTypes)
        {
            _guiSkinService = guiSkinService;
            _styleConverter = new ControlStyleJsonConverter(customControlTypes);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var skin = _guiSkinService.Skin;
            var style = (ControlStyle) _styleConverter.ReadJson(reader, objectType, existingValue, serializer);
            var template = GetControlTemplate(style);
            var control = skin.Create(style.TargetType, template) as ItemsControl;


            if (style.TryGetValue(nameof(ItemsControl.Items), out object childControls))
            {

                if (childControls is ControlCollection controlCollection)
                {
                    foreach (var child in controlCollection)
                        control.Items.Add(child);
                }
            }

            style.Apply(control);
            return control;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Control);
        }

        private static string GetControlTemplate(ControlStyle style)
        {

            if (style.TryGetValue(_styleProperty, out object template))
                return template as string;

            return null;
        }
    }
}