using System.Text.Json;
using System.Text.Json.Serialization;
namespace Services.JsonConvertSerializer
{
    public class StringToDecimalConverter : JsonConverter<decimal?>
    {
        public override bool HandleNull => true;

        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return Convert.ToDecimal(value);
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteStringValue(string.Empty);
            }
            else
            {
                writer.WriteNumberValue(value.Value);
            }
        }
    }
}
