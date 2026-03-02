using Newtonsoft.Json;

namespace Masofa.Common.Converters
{
    public class UtcDateTimeConverter : Newtonsoft.Json.JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            // При сериализации всегда пишем в UTC
            var utcValue = value.Kind == DateTimeKind.Utc
                ? value
                : value.ToUniversalTime();

            writer.WriteValue(utcValue);
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var temp = DateTime.Parse(reader.Value.ToString());

            // Приводим всё к UTC
            return temp.Kind switch
            {
                DateTimeKind.Utc => temp,
                DateTimeKind.Local => temp.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(temp, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(temp, DateTimeKind.Utc)
            };
        }
    }
}
