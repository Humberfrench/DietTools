using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dietcode.Core.Lib.JsonConverting
{

    public class FlexibleValueConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            var underlying = Nullable.GetUnderlyingType(typeToConvert);

            if (underlying == null)
                return false; // 🚫 NÃO converter tipos não-nullable

            return underlying == typeof(int)
                || underlying == typeof(long)
                || underlying == typeof(decimal)
                || underlying == typeof(double)
                || underlying == typeof(bool);
        }


        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeToConvert)!;

            var converterType = typeof(FlexibleValueConverter<>).MakeGenericType(underlyingType);

            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
