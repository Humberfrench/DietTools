using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        private static int maxDepth = 8;
        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = maxDepth,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,

        };

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, JsonSettings);
        }

        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, JsonSettings);
        }

        public static string SerializeObject(object value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, settings);
        }

        public static string Serialize<T>(T value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, settings);
        }

        public static T DeserializeObject<T>(string value) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(value, JsonSettings) ?? new T();
        }
        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(value, settings) ?? new T();
        }

        public static string ToJson(this object dado, JsonSerializerSettings settings)
        {
            return Serialize(dado, settings);
        }
        public static string ToJson(this object dado)
        {
            return Serialize(dado, JsonSettings);
        }

        public static string ToJson(this string dado)
        {
            return "{" + nameof(dado) + ":'" + dado + "'}";
        }

        #region Convert Objects

        /// <summary>
        /// Conversor de Objeto. 
        /// Atenção, o destino deve ter [json property] preenchido
        /// </summary>
        /// <typeparam name="Destiny">Objeto origem</typeparam>
        /// <typeparam name="Origin">Objeto destino</typeparam>
        /// <param name="data">objeto origem</param>
        /// <returns></returns>
        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data) where Destiny : new()
        {
            var json = SerializeObject(data);
            var retorno = new Destiny();
            try
            {
                retorno = DeserializeObject<Destiny>(json);
            }
            catch
            {
                return retorno;
            }

            return retorno;

        }
        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data, int profundidade) where Destiny : new()
        {
            JsonSettings.MaxDepth = profundidade;
            var json = SerializeObject(data, JsonSettings);
            var retorno = new Destiny();
            try
            {
                retorno = DeserializeObject<Destiny>(json, JsonSettings);
            }
            catch
            {
                JsonSettings.MaxDepth = maxDepth;
                return retorno;
            }

            JsonSettings.MaxDepth = maxDepth;

            return retorno;

        }
        public static Destiny ConvertObjects<Destiny, Origin>(this Origin data, JsonSerializerSettings settings) where Destiny : new()
        {
            var json = SerializeObject(data, settings);
            var retorno = new Destiny();
            try
            {
                retorno = DeserializeObject<Destiny>(json, settings);
            }
            catch
            {
                return retorno;
            }

            return retorno;

        }

        public static Destiny ConvertObjects<Destiny>(this object data) where Destiny : new()
        {
            var json = SerializeObject(data);
            var retorno = new Destiny();
            try
            {
                retorno = DeserializeObject<Destiny>(json);
            }
            catch
            {
                return retorno;
            }

            return retorno;

        }

        public static Destiny ConvertObjects<Destiny>(this object data, int profundidade) where Destiny : new()
        {
            JsonSettings.MaxDepth = profundidade;
            var json = SerializeObject(data, JsonSettings);
            var retorno = new Destiny();
            try
            {
                retorno = DeserializeObject<Destiny>(json, JsonSettings);
            }
            catch
            {
                JsonSettings.MaxDepth = maxDepth;
                return retorno;
            }

            JsonSettings.MaxDepth = maxDepth;

            return retorno;

        }
        public static Destiny ConvertObjects<Destiny>(this object data, JsonSerializerSettings settings) where Destiny : new()
        {
            var json = SerializeObject(data, settings);
            var retorno = new Destiny();
            try
            {
                retorno = DeserializeObject<Destiny>(json, settings);
            }
            catch
            {
                return retorno;
            }

            return retorno;

        }

        #endregion

    }
}
