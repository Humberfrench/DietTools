using System.Text.RegularExpressions;

namespace Dietcode.Core.Lib
{
    public static class Validacao
    {
        public static bool IsPis(string pis)
        {
            int[] multiplicador = new int[10] { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            if (pis.Trim().Length != 11)
                return false;
            pis = pis.Trim();
            pis = pis.Replace("-", "").Replace(".", "").PadLeft(11, '0');

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(pis[i].ToString()) * multiplicador[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            return pis.EndsWith(resto.ToString());
        }

        public static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }
        public static string DataRangeValido(string dataValidar)
        {
            var min = DateTime.Now.AddYears(-18);
            var max = DateTime.Now.AddYears(-100);
            var msg = $"Data Inválida, entrar com uma data entre {max:dd/MM/yyyy} e {min:dd/MM/yyyy}";

            try
            {
                var date = DateTime.Parse(dataValidar);

                if (date > min || date < max)
                {
                    return msg;
                }
                return "true";
            }
            catch (Exception)
            {
                return msg;
            }
        }
        public static string DataRangeValido(DateTime dataValidar)
        {
            var min = DateTime.Now.AddYears(-18);
            var max = DateTime.Now.AddYears(-100);
            var msg = $"Data Inválida, entrar com uma data entre {max:dd/MM/yyyy} e {min:dd/MM/yyyy}";

            try
            {
                if (dataValidar > min || dataValidar < max)
                {
                    return msg;
                }
                return "true";
            }
            catch (Exception)
            {
                return msg;
            }
        }
        public static string DataDocumentoRangeValido(string dataValidar, string dataNascimento)
        {
            var hoje = DateTime.Now;
            var msg = String.Empty;
            try
            {
                var date = DateTime.Parse(dataValidar);
                var dateNasc = DateTime.Parse(dataNascimento);
                msg = $"Data Inválida, entrar com uma data entre {dateNasc:dd/MM/yyyy} e {hoje:dd/MM/yyyy}";

                if (date > hoje || date < dateNasc)
                {
                    return msg;
                }
                return "true";
            }
            catch (Exception)
            {
                return msg;
            }
        }

        public static string DataDocumentoRangeValido(DateTime dataValidar, DateTime dataNascimento)
        {
            var hoje = DateTime.Now;
            var msg = String.Empty;
            try
            {
                msg = $"Data Inválida, entrar com uma data entre {dataNascimento:dd/MM/yyyy} e {hoje:dd/MM/yyyy}";

                if (dataValidar > hoje || dataValidar < dataNascimento)
                {
                    return msg;
                }
                return "true";
            }
            catch (Exception)
            {
                return msg;
            }
        }

        public static bool ValidarEmail(string email)
        {
            var rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            return rg.IsMatch(email);
        }

    }
}
