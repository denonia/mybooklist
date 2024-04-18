using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyBookList.Data;

internal class DescriptionJsonConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }
        
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var value = doc.RootElement.GetProperty("value");
            return JsonSerializer.Deserialize<T>(value, options);
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}