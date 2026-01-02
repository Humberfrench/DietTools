using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dietcode.Core.Lib.JsonConverting
{
    public static class JsonOptionsFactory
    {
        public static JsonSerializerOptions CreateDefault()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                MaxDepth = 128,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new FlexibleValueConverterFactory());
            options.Converters.Add(new FlexibleStringConverter());

            return options;
        }
    }
}
