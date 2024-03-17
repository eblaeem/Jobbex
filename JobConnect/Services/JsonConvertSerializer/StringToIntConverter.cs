using System.Text.Json;
using System.Text.Json.Serialization;
namespace Services.JsonConvertSerializer
{
    public class StringToIntConverter : JsonConverter<int?>
    {
        public override bool HandleNull => true;

        public override bool CanConvert(Type objectType)
        {
            objectType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            if (objectType == typeof(long)
            || objectType == typeof(ulong)
            || objectType == typeof(int)
            || objectType == typeof(uint)
            || objectType == typeof(short)
            || objectType == typeof(ushort)
            || objectType == typeof(byte)
            || objectType == typeof(sbyte)
            || objectType == typeof(System.Numerics.BigInteger)) { 
                return true;
            }
            return false;
        }

        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return Convert.ToInt32(value);
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
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
