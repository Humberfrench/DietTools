using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        // Mantém o mesmo valor, só deixei como const + readonly por segurança (não quebra nada)
        private const int maxDepth = 128;

        private static readonly JsonSerializerOptions defaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            MaxDepth = maxDepth,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // ============================
        // SERIALIZE
        // ============================

        public static string SerializeObject(object value, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(value, options);
        }

        public static string SerializeObject(object value)
        {
            return JsonSerializer.Serialize(value, defaultJsonOptions);
        }

        public static string Serialize<T>(T value, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(value, options);
        }

        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, defaultJsonOptions);
        }

        // ============================
        // DESERIALIZE
        // ============================

        public static T DeserializeObject<T>(string value, JsonSerializerOptions options) where T : new()
        {
            try
            {
                return JsonSerializer.Deserialize<T>(value, options) ?? new T();
            }
            catch (JsonException ex)
            {
                // mantém exatamente o padrão original:
                // lança InvalidOperationException com JsonException como InnerException
                throw new InvalidOperationException("Erro ao desserializar o objeto.", ex);
            }
        }

        public static T DeserializeObject<T>(string value) where T : new()
        {
            try
            {
                return JsonSerializer.Deserialize<T>(value, defaultJsonOptions) ?? new T();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Erro ao desserializar o objeto.", ex);
            }
        }

        // ============================
        // EXTENSÕES ToJson
        // ============================

        public static string ToJson(this object dado, JsonSerializerOptions options)
        {
            return Serialize(dado, options);
        }

        public static string ToJson(this object dado)
        {
            return Serialize(dado, defaultJsonOptions);
        }

        public static string ToJson(this string dado)
        {
            // mantém exatamente o comportamento antigo
            // {dado:'valor'} – mesmo que não seja JSON "padrão", pode ter consumo legado
            return "{" + nameof(dado) + ":'" + dado + "'}";
        }

        // ============================
        // CONVERT OBJECTS
        // ============================

        private static Destiny ConvertObjectsInternal<Destiny>(string json, JsonSerializerOptions options)
            where Destiny : new()
        {
            return DeserializeObject<Destiny>(json, options);
        }

        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data) where Destiny : new()
        {
            var json = SerializeObject(data!, defaultJsonOptions);
            return ConvertObjectsInternal<Destiny>(json, defaultJsonOptions);
        }

        public static Destiny ConvertObjects<Destiny>(this object data) where Destiny : new()
        {
            var json = SerializeObject(data, defaultJsonOptions);
            return ConvertObjectsInternal<Destiny>(json, defaultJsonOptions);
        }

        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data, JsonSerializerOptions options) where Destiny : new()
        {
            var json = SerializeObject(data!, options);
            return ConvertObjectsInternal<Destiny>(json, options);
        }

        public static Destiny ConvertObjects<Destiny>(this object data, JsonSerializerOptions options) where Destiny : new()
        {
            var json = SerializeObject(data, options);
            return ConvertObjectsInternal<Destiny>(json, options);
        }
    }
}
