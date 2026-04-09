using System.Text.Json;
using Dietcode.Core.Lib.JsonConverting;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        // Mantém o mesmo valor, só deixei como const + readonly por segurança (não quebra nada)
        private const int maxDepth = 128;
        private static readonly JsonSerializerOptions defaultJsonOptions = JsonOptionsFactory.CreateDefault();

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

        // ✅ Cópia que NÃO exige new() — funciona com struct / record / record class
        // Mantém o mesmo padrão de exceção (InvalidOperationException com JsonException como InnerException).
        public static T DeserializeObjectAny<T>(string value, JsonSerializerOptions options)
        {
            try
            {
                // Se não conseguir desserializar, retorna default(T):
                // - null para referência
                // - 0/false para struct
                return JsonSerializer.Deserialize<T>(value, options)!;
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

        public static T ToObject<T>(this string dado) where T : new()
        {
            T retorno = DeserializeObject<T>(dado, defaultJsonOptions) ?? new();
            return retorno;
        }

        public static T ToObject<T>(this string dado, JsonSerializerOptions options) where T : new()
        {
            T retorno = DeserializeObject<T>(dado, options) ?? new();
            return retorno;
        }

        // ============================
        // CONVERT OBJECTS
        // ============================

        private static Destiny ConvertObjectsInternal<Destiny>(string json, JsonSerializerOptions options)
            where Destiny : new()
        {
            return DeserializeObject<Destiny>(json, options);
        }

        // ✅ Cópia sem new() — suporta struct / record / record class
        private static Destiny ConvertObjectsInternalAny<Destiny>(string json, JsonSerializerOptions options)
        {
            var obj = DeserializeObjectAny<Destiny>(json, options);
            return obj!;
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

        // ✅ Cópias que NÃO exigem new() — funcionam com record posicional, record class, struct, etc.
        // Mantém seus métodos atuais intactos.

        /// <summary>
        /// Sem exigencia de New() — funciona com struct / record / record class
        /// </summary>
        /// <typeparam name="Destiny"></typeparam>
        /// <typeparam name="Origin"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Destiny ConvertObjectsAny<Destiny, Origin>(this Origin data)
        {
            var json = SerializeObject(data!, defaultJsonOptions);

            // Se o JSON não conseguir materializar Destiny, isso volta default(Destiny) (null p/ ref, 0 p/ struct).
            // Se você prefere falhar, troque por throw quando vier null.
            var obj = ConvertObjectsInternalAny<Destiny>(json, defaultJsonOptions);
            return obj!;
        }

        /// <summary>
        /// Sem exigencia de New() — funciona com struct / record / record class
        /// </summary>
        /// <typeparam name="Destiny"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Destiny ConvertObjectsAny<Destiny>(this object data)
        {
            var json = SerializeObject(data, defaultJsonOptions);

            var obj = ConvertObjectsInternalAny<Destiny>(json, defaultJsonOptions);
            return obj!;
        }

        /// <summary>
        /// Sem exigencia de New() — funciona com struct / record / record class
        /// </summary>
        /// <typeparam name="Destiny"></typeparam>
        /// <typeparam name="Origin"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Destiny ConvertObjectsAny<Destiny, Origin>(this Origin data, JsonSerializerOptions options)
        {
            var json = SerializeObject(data!, options);

            // Se o JSON não conseguir materializar Destiny, isso volta default(Destiny) (null p/ ref, 0 p/ struct).
            // Se você prefere falhar, troque por throw quando vier null.
            var obj = ConvertObjectsInternalAny<Destiny>(json, options);
            return obj!;
        }

        /// <summary>
        /// Sem exigencia de New() — funciona com struct / record / record class
        /// </summary>
        /// <typeparam name="Destiny"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Destiny ConvertObjectsAny<Destiny>(this object data, JsonSerializerOptions options)
        {
            var json = SerializeObject(data, options);

            var obj = ConvertObjectsInternalAny<Destiny>(json, options);
            return obj!;
        }

    }
}
