using System.Text.RegularExpressions;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        public static string GetFirstAndLastName(this string name)
        {

            var names = name.Split(' ');
            return $"{names.First()} {names.Last()}";

        }

        public static string GetFirstName(this string name)
        {

            var names = name.Split(' ');
            return names.First();

        }

        public static string GetLastName(this string name)
        {

            var names = name.Split(' ');
            return names.Last();

        }

        public static bool IsNullOrEmptyOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        public static int Int(this Enum enumer)
        {
            return Convert.ToInt32(enumer);
        }

        public static string TratarStringNull(this string value)
        {
            if (value.IsNullOrEmptyOrWhiteSpace())
            {
                return "";
            }

            return value;
        }

        public static string TratarStringNull(this int? value)
        {
            if (!value.HasValue)
            {
                return "";
            }

            if (value.Value == 0)
            {
                return "";
            }

            return value.ToString();
        }

        public static bool IsValidEmail(this string strIn)
        {
            //Retorna o e-mail quando valido
            return Regex.IsMatch(strIn,
                @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");

        }

        public static string FormatoCpfouCnpj(this string text)
        {
            //CNPJ
            if (text.Length == 14)
                return Convert.ToUInt64(text).ToString(@"00\.000\.000\/0000\-00");

            //CPF
            if (text.Length == 11)
                return Convert.ToUInt64(text).ToString(@"000\.000\.000\-00");

            return text;
        }

        public static string ToMoeda(this double? valor)
        {
            if (valor.HasValue)
            {
                return valor.Value.ToString("C");

            }
            return 0.ToString("C");
        }

        public static string ToMoeda(this decimal? valor)
        {
            if (valor.HasValue)
            {
                return valor.Value.ToString("C");

            }
            return 0.ToString("C");
        }

        public static string ToMoeda(this double valor)
        {
            return valor.ToString("C");
        }

        public static string ToMoeda(this decimal valor)
        {

            return valor.ToString("C");
        }

        public static string ToDateFormated(this DateTime dateValue)
        {
            return dateValue.ToString("dd/MM/yyyy");
        }

        public static string ToTimeFormated(this DateTime dateValue)
        {
            return dateValue.ToString("HH:mm");
        }

        public static string ToDateTimeFormated(this DateTime dateValue)
        {
            return $"{dateValue:dd/MM/yyyy} | {dateValue:HH:mm}";
        }

        public static string ToDateTimeWithSecondsFormated(this DateTime dateValue)
        {
            return $"{dateValue:dd/MM/yyyy} | {dateValue:HH:mm:ss}";
        }

        public static string ToDateTimeFormatedLinhaQuebrada(this DateTime? dateValue)
        {
            if (dateValue.HasValue)
            {
                return $"{dateValue.Value:dd/MM/yyyy} <br /> {dateValue.Value:HH:mm}";
            }
            return "";
        }

        public static string ToDateTimeFormatedLinhaQuebrada(this DateTime dateValue)
        {
            return $"{dateValue:dd/MM/yyyy} <br /> {dateValue:HH:mm}";
        }

        public static string ToDateTimeFormated(this DateTime dateValue, string separador)
        {
            return $"{dateValue:dd/MM/yyyy} {separador} {dateValue:HH:mm}";
        }

        public static string ToDateTimeWithSecondsFormated(this DateTime dateValue, string separador)
        {
            return $"{dateValue:dd/MM/yyyy} {separador} {dateValue:HH:mm:ss}";
        }

        public static string ToDateFormated(this DateTime? dateValue)
        {
            if (dateValue.HasValue)
            {
                return dateValue.Value.ToString("dd/MM/yyyy");
            }
            return "";
        }

        public static string ToTimeFormated(this DateTime? dateValue)
        {
            if (dateValue.HasValue)
            {
                return dateValue.Value.ToString("HH:mm");
            }
            return "";
        }

        public static string ToDateTimeFormated(this DateTime? dateValue)
        {
            if (dateValue.HasValue)
            {
                return $"{dateValue.Value:dd/MM/yyyy} | {dateValue.Value:HH:mm}";
            }
            return "";
        }

        public static string ToDateTimeFormated(this DateTime? dateValue, string separador)
        {
            if (dateValue.HasValue)
            {
                return $"{dateValue.Value:dd/MM/yyyy} {separador} {dateValue.Value:HH:mm}";
            }
            return "";
        }

        public static string ToDateTimeWithSecondsFormated(this DateTime? dateValue)
        {
            if (dateValue.HasValue)
            {
                return $"{dateValue.Value:dd/MM/yyyy} | {dateValue.Value:HH:mm:ss}";
            }
            return "";
        }

        public static string ToDateTimeWithSecondsFormated(this DateTime? dateValue, string separador)
        {
            if (dateValue.HasValue)
            {
                return $"{dateValue.Value:dd/MM/yyyy} {separador} {dateValue.Value:HH:mm:ss}";
            }
            return "";
        }

        public static string ToSimNao(this string stringValue)
        {
            if (stringValue == "S")
            {
                return "Sim";
            }
            return "Não";
        }

        public static string ToSimNao(this bool boolValue)
        {
            if (boolValue)
            {
                return "Sim";
            }
            return "Não";
        }

        public static string ToSimNao(this bool? boolValue)
        {
            if (boolValue.HasValue)
            {
                if (boolValue.Value)
                {
                    return "Sim";
                }
                return "Não";
            }
            return "Não";
        }



    }
}
