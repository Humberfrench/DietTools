using System.Text.Json;

namespace Dietcode.Database.Logging
{
    internal static class JsonLogOptions
    {
        public static readonly JsonSerializerOptions Default =
            new()
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
    }
}
