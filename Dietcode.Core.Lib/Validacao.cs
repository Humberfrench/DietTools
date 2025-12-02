using System.Text.RegularExpressions;

namespace Dietcode.Core.Lib
{
    public static class Validacao
    {
        private static readonly Regex DigitsOnly = new(@"^\d+$", RegexOptions.Compiled);

        // -----------------------------
        // DOCUMENTOS
        // -----------------------------

        public static string CorrigirDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return documento ?? string.Empty;

            var cleaned = Limpar(documento);

            return cleaned.Length switch
            {
                11 or 14 => cleaned,
                < 11 => cleaned.PadLeft(11, '0'), // CPF
                < 14 => cleaned.PadLeft(14, '0'), // CNPJ
                _ => cleaned
            };
        }

        public static bool IsPis(string pis)
        {
            pis = Limpar(pis).PadLeft(11, '0');

            if (pis.Length != 11 || !DigitsOnly.IsMatch(pis))
                return false;

            int[] pesos = { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (pis[i] - '0') * pesos[i];

            int resto = soma % 11;
            int digito = resto < 2 ? 0 : 11 - resto;

            return pis.EndsWith(digito.ToString());
        }

        public static bool IsCpf(string cpf)
        {
            cpf = Limpar(cpf);

            if (cpf.Length != 11 || !DigitsOnly.IsMatch(cpf))
                return false;

            if (TodosDigitosIguais(cpf))
                return false;

            string temp = cpf[..9];

            int dig1 = CalcularDigitoCpf(temp, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 });
            temp += dig1;

            int dig2 = CalcularDigitoCpf(temp, new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 });

            return cpf.EndsWith($"{dig1}{dig2}");
        }

        public static bool IsCnpj(string cnpj)
        {
            cnpj = Limpar(cnpj);

            if (cnpj.Length != 14 || !DigitsOnly.IsMatch(cnpj))
                return false;

            if (TodosDigitosIguais(cnpj))
                return false;

            string temp = cnpj[..12];

            int dig1 = CalcularDigitoCnpj(temp, new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
            temp += dig1;

            int dig2 = CalcularDigitoCnpj(temp, new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

            return cnpj.EndsWith($"{dig1}{dig2}");
        }

        // -----------------------------
        // DATAS
        // -----------------------------

        public static string DataRangeValido(string dataValidar)
        {
            if (!DateTime.TryParse(dataValidar, out var data))
                return RangeMsg(DateTime.Now.AddYears(-100), DateTime.Now.AddYears(-18));

            return DataRangeValido(data);
        }

        public static string DataRangeValido(DateTime dataValidar)
        {
            var min = DateTime.Now.AddYears(-18); // pessoa mais jovem
            var max = DateTime.Now.AddYears(-100); // pessoa mais velha

            return (dataValidar >= max && dataValidar <= min)
                ? "true"
                : RangeMsg(max, min);
        }

        public static string DataDocumentoRangeValido(string dataValidar, string dataNascimento)
        {
            if (!DateTime.TryParse(dataValidar, out var dv))
                return "Data inválida";

            if (!DateTime.TryParse(dataNascimento, out var dn))
                return "Data de nascimento inválida";

            return DataDocumentoRangeValido(dv, dn);
        }

        public static string DataDocumentoRangeValido(DateTime dataValidar, DateTime dataNascimento)
        {
            var hoje = DateTime.Now;

            if (dataValidar < dataNascimento || dataValidar > hoje)
                return RangeMsg(dataNascimento, hoje);

            return "true";
        }

        // -----------------------------
        // HELPERS
        // -----------------------------

        private static string Limpar(string s) =>
            s?.Replace(".", "")
              .Replace("-", "")
              .Replace("/", "")
              .Replace(" ", "") ?? string.Empty;

        private static bool TodosDigitosIguais(string s) =>
            s.Distinct().Count() == 1;

        private static int CalcularDigitoCpf(string baseCpf, int[] pesos)
        {
            int soma = 0;

            for (int i = 0; i < pesos.Length; i++)
                soma += (baseCpf[i] - '0') * pesos[i];

            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static int CalcularDigitoCnpj(string baseCnpj, int[] pesos)
        {
            int soma = 0;

            for (int i = 0; i < pesos.Length; i++)
                soma += (baseCnpj[i] - '0') * pesos[i];

            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static string RangeMsg(DateTime min, DateTime max) =>
            $"Data Inválida, entrar com uma data entre {min:dd/MM/yyyy} e {max:dd/MM/yyyy}";
    }
}
