using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dietcode.Classic.Lib
{
    public static partial class Extensions
    {
        private const int maxDepth = 128;
        private static readonly JsonSerializerSettings defaultJsonSettings = CreateDefaultJsonSettings();

        private static JsonSerializerSettings CreateDefaultJsonSettings()
        {
            return new JsonSerializerSettings
            {
                MaxDepth = maxDepth,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
        }

        public static string SerializeObject(object value, JsonSerializerSettings options)
        {
            return JsonConvert.SerializeObject(value, options);
        }

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, defaultJsonSettings);
        }

        public static string Serialize<T>(T value, JsonSerializerSettings options)
        {
            return JsonConvert.SerializeObject(value, options);
        }

        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, defaultJsonSettings);
        }

        public static T DeserializeObject<T>(string value, JsonSerializerSettings options) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value, options) ?? new T();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Erro ao desserializar o objeto.", ex);
            }
        }

        public static T DeserializeObjectAny<T>(string value, JsonSerializerSettings options)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value, options)!;
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
                return JsonConvert.DeserializeObject<T>(value, defaultJsonSettings) ?? new T();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Erro ao desserializar o objeto.", ex);
            }
        }

        public static string ToJson(this object dado, JsonSerializerSettings options)
        {
            return Serialize(dado, options);
        }

        public static string ToJson(this object dado)
        {
            return Serialize(dado, defaultJsonSettings);
        }

        public static string ToJson(this string dado)
        {
            return "{" + nameof(dado) + ":'" + dado + "'}";
        }

        public static T ToObject<T>(this string dado) where T : new()
        {
            return DeserializeObject<T>(dado, defaultJsonSettings);
        }

        public static T ToObject<T>(this string dado, JsonSerializerSettings options) where T : new()
        {
            return DeserializeObject<T>(dado, options);
        }

        public static JObject ToJObject(this string json)
        {
            return JObject.Parse(json);
        }

        public static JToken ToJToken(this string json)
        {
            return JToken.Parse(json);
        }

        public static JObject ToJObject(this object value)
        {
            if (value is JObject jObject)
                return (JObject)jObject.DeepClone();

            return JObject.FromObject(value, JsonSerializer.Create(defaultJsonSettings));
        }

        public static JToken ToJToken(this object value)
        {
            if (value == null)
                return JValue.CreateNull();

            if (value is JToken token)
                return token.DeepClone();

            return JToken.FromObject(value, JsonSerializer.Create(defaultJsonSettings));
        }

        private static Destiny ConvertObjectsInternal<Destiny>(string json, JsonSerializerSettings options)
            where Destiny : new()
        {
            return DeserializeObject<Destiny>(json, options);
        }

        private static Destiny ConvertObjectsInternalAny<Destiny>(string json, JsonSerializerSettings options)
        {
            var obj = DeserializeObjectAny<Destiny>(json, options);
            return obj!;
        }

        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data) where Destiny : new()
        {
            var json = SerializeObject(data!, defaultJsonSettings);
            return ConvertObjectsInternal<Destiny>(json, defaultJsonSettings);
        }

        public static Destiny ConvertObjects<Destiny>(this object data) where Destiny : new()
        {
            var json = SerializeObject(data, defaultJsonSettings);
            return ConvertObjectsInternal<Destiny>(json, defaultJsonSettings);
        }

        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data, JsonSerializerSettings options) where Destiny : new()
        {
            var json = SerializeObject(data!, options);
            return ConvertObjectsInternal<Destiny>(json, options);
        }

        public static Destiny ConvertObjects<Destiny>(this object data, JsonSerializerSettings options) where Destiny : new()
        {
            var json = SerializeObject(data, options);
            return ConvertObjectsInternal<Destiny>(json, options);
        }

        public static Destiny ConvertObjectsAny<Destiny, Origin>(this Origin data)
        {
            var json = SerializeObject(data!, defaultJsonSettings);
            return ConvertObjectsInternalAny<Destiny>(json, defaultJsonSettings)!;
        }

        public static Destiny ConvertObjectsAny<Destiny>(this object data)
        {
            var json = SerializeObject(data, defaultJsonSettings);
            return ConvertObjectsInternalAny<Destiny>(json, defaultJsonSettings)!;
        }

        public static Destiny ConvertObjectsAny<Destiny, Origin>(this Origin data, JsonSerializerSettings options)
        {
            var json = SerializeObject(data!, options);
            return ConvertObjectsInternalAny<Destiny>(json, options)!;
        }

        public static Destiny ConvertObjectsAny<Destiny>(this object data, JsonSerializerSettings options)
        {
            var json = SerializeObject(data, options);
            return ConvertObjectsInternalAny<Destiny>(json, options)!;
        }
    }
}
