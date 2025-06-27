using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        private static int maxDepth = 8;
        private static JsonSerializerOptions defaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = maxDepth,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

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

        public static T DeserializeObject<T>(string value, JsonSerializerOptions options) where T : new()
        {
            try
            {
                return JsonSerializer.Deserialize<T>(value, options) ?? new T();
            }
            catch (JsonException ex)
            {
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
            return "{" + nameof(dado) + ":'" + dado + "'}";
        }

        #region Convert Objects

        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data) where Destiny : new()
        {
            var json = SerializeObject(data!, defaultJsonOptions);
            return DeserializeObject<Destiny>(json, defaultJsonOptions);
        }

        public static Destiny ConvertObjects<Destiny>(this object data) where Destiny : new()
        {
            var json = SerializeObject(data, defaultJsonOptions);
            return DeserializeObject<Destiny>(json, defaultJsonOptions);
        }
        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data, JsonSerializerOptions options) where Destiny : new()
        {
            var json = SerializeObject(data!, options);
            return DeserializeObject<Destiny>(json, options);
        }

        public static Destiny ConvertObjects<Destiny>(this object data, JsonSerializerOptions options) where Destiny : new()
        {
            var json = SerializeObject(data, options);
            return DeserializeObject<Destiny>(json, options);
        }

        #endregion
    }
}
