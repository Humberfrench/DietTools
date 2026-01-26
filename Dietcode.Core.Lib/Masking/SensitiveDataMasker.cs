using System.Text.Json;

namespace Dietcode.Core.Lib.Masking
{
    public static class SensitiveDataMasker
    {
        private static readonly HashSet<string> SensitiveFields =
            new(StringComparer.OrdinalIgnoreCase)
            {
            "password",
            "senha",
            "token",
            "accessToken",
            "refreshToken",
            "authorization",
            "apiKey"
            };

        public static object? Mask(object? data)
        {
            if (data == null)
                return null;

            // 🛑 CASO 1: string
            if (data is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return str;

                str = str.TrimStart();

                // Não parece JSON → retorna como texto cru
                if (!(str.StartsWith("{") || str.StartsWith("[")))
                    return str;

                // Parece JSON → tenta mascarar
                try
                {
                    using var doc = JsonDocument.Parse(str);
                    var masked = MaskElement(doc.RootElement);
                    return JsonSerializer.Deserialize<object>(masked.GetRawText());
                }
                catch
                {
                    return str; // fallback total
                }
            }

            // 🛑 CASO 2: objeto normal
            try
            {
                var json = JsonSerializer.Serialize(data);
                using var doc = JsonDocument.Parse(json);

                var masked = MaskElement(doc.RootElement);

                return JsonSerializer.Deserialize<object>(
                    masked.GetRawText());
            }
            catch
            {
                // Nunca quebrar o pipeline
                return data;
            }
        }

        private static JsonElement MaskElement(JsonElement element)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            WriteMasked(element, writer);

            writer.Flush();
            stream.Position = 0;

            return JsonDocument.Parse(stream).RootElement.Clone();
        }

        private static void WriteMasked(JsonElement element, Utf8JsonWriter writer)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    writer.WriteStartObject();
                    foreach (var prop in element.EnumerateObject())
                    {
                        writer.WritePropertyName(prop.Name);

                        if (SensitiveFields.Contains(prop.Name))
                        {
                            writer.WriteStringValue("***");
                        }
                        else
                        {
                            WriteMasked(prop.Value, writer);
                        }
                    }
                    writer.WriteEndObject();
                    break;

                case JsonValueKind.Array:
                    writer.WriteStartArray();
                    foreach (var item in element.EnumerateArray())
                        WriteMasked(item, writer);
                    writer.WriteEndArray();
                    break;

                default:
                    element.WriteTo(writer);
                    break;
            }
        }
    }
}
