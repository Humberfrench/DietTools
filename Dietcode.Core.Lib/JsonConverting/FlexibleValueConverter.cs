using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dietcode.Core.Lib.JsonConverting
{
    public class FlexibleValueConverter<T> : JsonConverter<T?>
        where T : struct
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();

                if (string.IsNullOrWhiteSpace(s))
                    return null;

                return ConvertFromString(s);
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return ConvertFromNumber(reader);
            }

            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                if (typeof(T) == typeof(bool))
                    return (T)(object)(reader.TokenType == JsonTokenType.True);
            }

            throw new JsonException($"Não foi possível converter {reader.TokenType} para {typeof(T).Name}");
        }

        private static T? ConvertFromString(string value)
        {
            try
            {
                if (typeof(T) == typeof(bool))
                {
                    if (bool.TryParse(value, out var b))
                        return (T)(object)b;

                    if (int.TryParse(value, out var i))
                        return (T)(object)(i != 0);
                }

                return (T?)Convert.ChangeType(
                    value,
                    typeof(T),
                    CultureInfo.InvariantCulture
                );
            }
            catch
            {
                return null;
            }
        }

        private static T ConvertFromNumber(Utf8JsonReader reader)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)reader.GetInt32();

            if (typeof(T) == typeof(long))
                return (T)(object)reader.GetInt64();

            if (typeof(T) == typeof(decimal))
                return (T)(object)reader.GetDecimal();

            if (typeof(T) == typeof(double))
                return (T)(object)reader.GetDouble();

            if (typeof(T) == typeof(bool))
                return (T)(object)(reader.GetInt32() != 0);

            throw new JsonException($"Tipo {typeof(T).Name} não suportado");
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            if (value.Value is bool b)
                writer.WriteBooleanValue(b);
            else if (value.Value is int i)
                writer.WriteNumberValue(i);
            else if (value.Value is long l)
                writer.WriteNumberValue(l);
            else if (value.Value is decimal d)
                writer.WriteNumberValue(d);
            else if (value.Value is double db)
                writer.WriteNumberValue(db);
            else
                throw new JsonException("Tipo não suportado na escrita");
        }
    }
}
