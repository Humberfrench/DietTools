using Newtonsoft.Json.Linq;

namespace Dietcode.Classic.Lib.Masking
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

            if (data is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return str;

                str = str.TrimStart();

                if (!(str.StartsWith("{") || str.StartsWith("[")))
                    return str;

                try
                {
                    var token = JToken.Parse(str);
                    var masked = MaskToken(token);
                    return masked.ToObject<object>();
                }
                catch
                {
                    return str;
                }
            }

            try
            {
                var token = data as JToken ?? JToken.FromObject(data);
                var masked = MaskToken(token);
                return masked.ToObject<object>();
            }
            catch
            {
                return data;
            }
        }

        private static JToken MaskToken(JToken token)
        {
            var clone = token.DeepClone();
            MaskTokenInPlace(clone);
            return clone;
        }

        private static void MaskTokenInPlace(JToken token)
        {
            if (token is JObject obj)
            {
                foreach (var prop in obj.Properties().ToList())
                {
                    if (SensitiveFields.Contains(prop.Name))
                        prop.Value = "***";
                    else
                        MaskTokenInPlace(prop.Value);
                }

                return;
            }

            if (token is JArray array)
            {
                foreach (var item in array)
                    MaskTokenInPlace(item);
            }
        }
    }
}
