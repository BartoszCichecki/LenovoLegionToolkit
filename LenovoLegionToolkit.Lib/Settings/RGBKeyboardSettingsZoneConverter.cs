using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LenovoLegionToolkit.Lib.Settings
{
    internal class RGBKeyboardSettingsZoneConverter : JsonConverter<RGBKeyboardZone?>
    {
        public override bool CanWrite => false;

        public override RGBKeyboardZone? ReadJson(JsonReader reader, Type objectType, RGBKeyboardZone? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = serializer.Deserialize<JObject>(reader);

            if (jObject == null)
                return null;

            bool jObjectIsRgbColor = jObject.Property("R") != null
                                      && jObject.Property("G") != null
                                      && jObject.Property("B") != null;

            if (jObjectIsRgbColor)
                return new(jObject.ToObject<RGBColor>(), false);
            else
                return jObject.ToObject<RGBKeyboardZone>();
        }

        public override void WriteJson(JsonWriter writer, RGBKeyboardZone? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
