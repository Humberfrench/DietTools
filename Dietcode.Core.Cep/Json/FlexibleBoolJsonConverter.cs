using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dietcode.Core.Cep.Json;

internal sealed class FlexibleBoolJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.True or JsonTokenType.False)
            return reader.GetBoolean();

        if (reader.TokenType == JsonTokenType.String)
            return bool.TryParse(reader.GetString(), out var value) && value;

        return false;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
