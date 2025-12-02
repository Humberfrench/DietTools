using System.Text;
using System.Text.RegularExpressions;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        // --------------------------------------------------
        //  STRINGS (Nomes)
        // --------------------------------------------------

        public static string GetFirstAndLastName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var names = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (names.Length == 1)
                return names[0];

            return $"{names.First()} {names.Last()}";
        }

        public static string GetFirstName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return name.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
        }

        public static string GetLastName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return name.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty;
        }
        public static string ToSnakeCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var snake = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1_$2");
            return snake.ToLowerInvariant();
        }
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var parts = value.Split(new[] { ' ', '_', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length == 0
                ? value
                : parts[0].ToLowerInvariant() +
                  string.Concat(parts.Skip(1).Select(p =>
                      char.ToUpperInvariant(p[0]) + p.Substring(1).ToLowerInvariant()));
        }
        public static string ToKebabCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var kebab = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1-$2");
            return kebab.ToLowerInvariant();
        }

        public static string ToSnakeCaseSpan(this ReadOnlySpan<char> value)
        {
            if (value.IsEmpty) return string.Empty;

            var output = new StringBuilder(value.Length * 2);

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (i > 0 && char.IsUpper(c) && char.IsLower(value[i - 1]))
                    output.Append('_');

                output.Append(char.ToLowerInvariant(c));
            }

            return output.ToString();
        }
        public static string ToKebabCaseSpan(this ReadOnlySpan<char> value)
        {
            if (value.IsEmpty) return string.Empty;

            var output = new StringBuilder(value.Length * 2);

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (i > 0 && char.IsUpper(c) && char.IsLower(value[i - 1]))
                    output.Append('-');

                output.Append(char.ToLowerInvariant(c));
            }

            return output.ToString();
        }
        public static string ToCamelCaseSpan(this ReadOnlySpan<char> value)
        {
            if (value.IsEmpty) return string.Empty;

            var parts = value
                .ToString()
                .Split(new[] { ' ', '_', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();

            sb.Append(parts[0].ToLowerInvariant());

            for (int i = 1; i < parts.Length; i++)
            {
                var p = parts[i];
                sb.Append(char.ToUpperInvariant(p[0]));
                if (p.Length > 1)
                    sb.Append(p.Substring(1).ToLowerInvariant());
            }

            return sb.ToString();
        }

        // --------------------------------------------------
        //  STRINGS (Validações)
        // --------------------------------------------------

        public static bool HasLetters(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                    return true;
            }
            return false;
        }

        public static bool IsNullOrEmptyOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        // --------------------------------------------------
        //  ENUM
        // --------------------------------------------------

        public static int Int(this Enum enumer)
        {
            return Convert.ToInt32(enumer);
        }

        // --------------------------------------------------
        //  STRINGS - Normalização
        // --------------------------------------------------

        public static string TratarStringNull(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value;
        }

        public static string TratarStringNull(this int? value)
        {
            return (!value.HasValue || value.Value == 0)
                ? ""
                : value.Value.ToString();
        }

        // --------------------------------------------------
        //  EMAIL
        // --------------------------------------------------

        private static readonly Regex EmailRegex = new(
            @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$",
            RegexOptions.Compiled);

        public static bool IsValidEmail(this string strIn)
        {
            if (string.IsNullOrWhiteSpace(strIn))
                return false;

            return EmailRegex.IsMatch(strIn.Trim());
        }

        // --------------------------------------------------
        //  MOEDA
        // --------------------------------------------------

        public static string ToMoeda(this double? valor)
            => valor.HasValue ? valor.Value.ToString("C") : 0.ToString("C");

        public static string ToMoeda(this decimal? valor)
            => valor.HasValue ? valor.Value.ToString("C") : 0.ToString("C");

        public static string ToMoeda(this double valor)
            => valor.ToString("C");

        public static string ToMoeda(this decimal valor)
            => valor.ToString("C");

        // --------------------------------------------------
        //  DATAS E HORAS
        // --------------------------------------------------

        public static string ToDateFormated(this DateTime dateValue)
            => dateValue.ToString("dd/MM/yyyy");

        public static string ToTimeFormated(this DateTime dateValue)
            => dateValue.ToString("HH:mm");

        public static string ToDateTimeFormated(this DateTime dateValue)
            => $"{dateValue:dd/MM/yyyy} | {dateValue:HH:mm}";

        public static string ToDateTimeWithSecondsFormated(this DateTime dateValue)
            => $"{dateValue:dd/MM/yyyy} | {dateValue:HH:mm:ss}";

        public static string ToDateFormated(this DateTime? dateValue)
            => dateValue?.ToString("dd/MM/yyyy") ?? "";

        public static string ToTimeFormated(this DateTime? dateValue)
            => dateValue?.ToString("HH:mm") ?? "";

        public static string ToDateTimeFormated(this DateTime? dateValue)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} | {dateValue.Value:HH:mm}"
                : "";

        public static string ToDateTimeWithSecondsFormated(this DateTime? dateValue)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} | {dateValue.Value:HH:mm:ss}"
                : "";

        public static string ToDateTimeFormated(this DateTime dateValue, string separador)
            => $"{dateValue:dd/MM/yyyy} {separador} {dateValue:HH:mm}";

        public static string ToDateTimeWithSecondsFormated(this DateTime dateValue, string separador)
            => $"{dateValue:dd/MM/yyyy} {separador} {dateValue:HH:mm:ss}";

        public static string ToDateTimeFormated(this DateTime? dateValue, string separador)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} {separador} {dateValue.Value:HH:mm}"
                : "";

        public static string ToDateTimeWithSecondsFormated(this DateTime? dateValue, string separador)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} {separador} {dateValue.Value:HH:mm:ss}"
                : "";

        // --------------------------------------------------
        //  SIM / NÃO
        // --------------------------------------------------

        public static string ToSimNao(this string stringValue)
            => stringValue == "S" ? "Sim" : "Não";

        public static string ToSimNao(this bool boolValue)
            => boolValue ? "Sim" : "Não";

        public static string ToSimNao(this bool? boolValue)
            => boolValue.HasValue && boolValue.Value ? "Sim" : "Não";

        // --------------------------------------------------
        //  phones
        // --------------------------------------------------
        public static string ToPhoneFormated(this string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            var digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.Length == 10)
            {
                // Formato: (11) 2345-6789
                return $"({digits[..2]}) {digits.Substring(2, 4)}-{digits.Substring(6, 4)}";
            }

            if (digits.Length == 11)
            {
                // Formato: (11) 91234-5678
                return $"({digits[..2]}) {digits.Substring(2, 5)}-{digits.Substring(7, 4)}";
            }

            return phone; // mantém original caso não seja 10 ou 11 dígitos
        }
        public static string ToPhoneFormatedSpan(this ReadOnlySpan<char> value)
        {
            Span<char> digits = stackalloc char[value.Length];
            int d = 0;

            foreach (var c in value)
                if (char.IsDigit(c))
                    digits[d++] = c;

            if (d == 10)
                return $"({new string(digits[..2])}) {new string(digits.Slice(2, 4))}-{new string(digits.Slice(6, 4))}";

            if (d == 11)
                return $"({new string(digits[..2])}) {new string(digits.Slice(2, 5))}-{new string(digits.Slice(7, 4))}";

            return value.ToString();
        }

    }
}

