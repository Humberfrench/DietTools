namespace Dietcode.Core.Lib
{
    public static class Documento
    {
        private static readonly string[] RemoverChars = { ".", "-", "/", " " };

        // =====================================================================
        //  REMOVER FORMATACAO
        // =====================================================================
        public static string SemFormatacao(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return string.Empty;

            string result = documento;

            foreach (var c in RemoverChars)
                result = result.Replace(c, string.Empty);

            return result.Trim();
        }

        // =====================================================================
        //  TRATAR DOCUMENTO (AUTO CPF/CNPJ)
        // =====================================================================
        public static string TratarDocumento(string documento)
        {
            var doc = SemFormatacao(documento);

            return doc.Length switch
            {
                11 => TratarDocumentoCpf(doc),
                14 => TratarDocumentoCnpj(doc),
                _ => doc
            };
        }

        // =====================================================================
        //  CPF (NORMAL)
        // =====================================================================
        public static string TratarDocumentoCpf(string documento)
        {
            var doc = SemFormatacao(documento);

            return doc.Length == 11 && doc.All(char.IsDigit)
                ? Convert.ToUInt64(doc).ToString(@"000\.000\.000\-00")
                : doc;
        }

        // =====================================================================
        //  CNPJ (NORMAL)
        // =====================================================================
        public static string TratarDocumentoCnpj(string documento)
        {
            var doc = SemFormatacao(documento);

            return doc.Length == 14 && doc.All(char.IsDigit)
                ? Convert.ToUInt64(doc).ToString(@"00\.000\.000\/0000\-00")
                : doc;
        }

        // =====================================================================
        //  CPF/CNPJ NORMAL - EXTENSÕES EM STRING
        // =====================================================================
        public static string ToCpf(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            var digits = new string(value.Where(char.IsDigit).ToArray());
            return digits.Length == 11
                ? Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00")
                : value;
        }

        public static string ToCnpj(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            var digits = new string(value.Where(char.IsDigit).ToArray());
            return digits.Length == 14
                ? Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00")
                : value;
        }

        public static string FormatoCpfouCnpj(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            var digits = new string(text.Where(char.IsDigit).ToArray());

            return digits.Length switch
            {
                11 => Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00"),
                14 => Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00"),
                _ => text
            };
        }

        // =====================================================================
        //  CPF/CNPJ — VERSÕES PERFORMÁTICAS (SPAN)
        // =====================================================================
        public static string ToCpfSpan(this ReadOnlySpan<char> value)
        {
            Span<char> digits = stackalloc char[value.Length];
            int d = 0;

            foreach (var c in value)
                if (char.IsDigit(c))
                    digits[d++] = c;

            if (d != 11)
                return value.ToString();

            return Convert.ToUInt64(new string(digits[..11]))
                .ToString(@"000\.000\.000\-00");
        }

        public static string ToCnpjSpan(this ReadOnlySpan<char> value)
        {
            Span<char> digits = stackalloc char[value.Length];
            int d = 0;

            foreach (var c in value)
                if (char.IsDigit(c))
                    digits[d++] = c;

            if (d != 14)
                return value.ToString();

            return Convert.ToUInt64(new string(digits[..14]))
                .ToString(@"00\.000\.000\/0000\-00");
        }

        // =====================================================================
        //  VALIDADORES
        // =====================================================================

        // -------------------- RG --------------------
        public static bool IsValidRg(this string rg)
        {
            if (string.IsNullOrWhiteSpace(rg))
                return false;

            var digits = new string(rg.Where(char.IsDigit).ToArray());
            if (digits.Length is < 8 or > 9)
                return false;

            int sum = 0;
            int weight = digits.Length == 8 ? 2 : 3;

            for (int i = 0; i < digits.Length - 1; i++)
                sum += (digits[i] - '0') * (weight + i);

            int dv = sum % 11;
            if (dv == 10) dv = 0;

            return dv == (digits[^1] - '0');
        }

        // -------------------- CNH --------------------
        public static bool IsValidCnh(this string cnh)
        {
            if (string.IsNullOrWhiteSpace(cnh))
                return false;

            var digits = new string(cnh.Where(char.IsDigit).ToArray());
            if (digits.Length != 11)
                return false;

            int dsc = 0;
            int v1 = 0;

            for (int i = 0, j = 9; i < 9; i++, j--)
                v1 += (digits[i] - '0') * j;

            int dv1 = v1 % 11;
            if (dv1 >= 10)
            {
                dv1 = 0;
                dsc = 2;
            }

            if ((digits[9] - '0') != dv1)
                return false;

            int v2 = 0;

            for (int i = 0, j = 1; i < 9; i++, j++)
                v2 += (digits[i] - '0') * j;

            int dv2 = v2 % 11;
            if (dv2 >= 10) dv2 = 0;

            return (digits[10] - '0') == (dv2 - dsc);
        }

        // -------------------- RENAVAM --------------------
        public static bool IsValidRenavam(this string renavam)
        {
            if (string.IsNullOrWhiteSpace(renavam))
                return false;

            var digits = new string(renavam.Where(char.IsDigit).ToArray());
            if (digits.Length != 11)
                return false;

            var body = digits[..10];
            int sum = 0;

            int[] weights = { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            for (int i = 0; i < 10; i++)
                sum += (body[i] - '0') * weights[i];

            int dv = 11 - (sum % 11);
            if (dv >= 10) dv = 0;

            return dv == (digits[10] - '0');
        }
    }
}
