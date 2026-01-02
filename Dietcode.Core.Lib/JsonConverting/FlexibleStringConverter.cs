using System.Text.Json;
using System.Text.Json.Serialization;
namespace Dietcode.Core.Lib.JsonConverting
{
    public class FlexibleStringConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString();

            // Tudo que NÃO for string vira null
            if (reader.TokenType == JsonTokenType.Null ||
                reader.TokenType == JsonTokenType.True ||
                reader.TokenType == JsonTokenType.False ||
                reader.TokenType == JsonTokenType.Number ||
                reader.TokenType == JsonTokenType.StartObject ||
                reader.TokenType == JsonTokenType.StartArray)
            {
                reader.Skip(); // consome o token corretamente
                return null;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value);
        }
    }
}
