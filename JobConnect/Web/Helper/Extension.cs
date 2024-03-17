using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.Text.Json.Serialization;
using ViewModel;

namespace Api.Helper
{

    public static class HelperExtension
    {
        public static string Errors(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
                return null;

            var list = modelState.Where(ms => ms.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors }).ToList();
            var errors = (from state in list
                          from modelError in state.Errors
                          select new LabelValue
                          {
                              //Value = state.Key,
                              Label = modelError.ErrorMessage.ToLower()
                                  .Replace("the", "")
                                  .Replace("field is required.", "الزامی میباشد.")
                          }).ToList();
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(errors.ToList());
            return data;
        }
    }
    public class AutoNumberToStringConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string) == typeToConvert;
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ?
                    l.ToString() :
                    reader.GetDouble().ToString();
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return document.RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
