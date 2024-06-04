namespace Dietcode.Core.Lib
{
    public static class BoletoValidator
    {

        public static string IdentificarTipoCodigo(string codigo)
        {
            codigo = codigo.TratarCodigo();

            if (codigo.Length == 44)
            {
                return "CODIGO_DE_BARRAS";
            }
            else if (codigo.Length == 46 || codigo.Length == 47 || codigo.Length == 48)
            {
                return "LINHA_DIGITAVEL";
            }
            else
            {
                return "TAMANHO_INCORRETO";
            }
        }

        public static EnumTipoCodigo IdentificarTipoDeCodigo(string codigo)
        {
            codigo = codigo.TratarCodigo();

            if (codigo.Length == 44)
            {
                return EnumTipoCodigo.CodigoDeBarras;
            }
            else if (codigo.Length == 46 || codigo.Length == 47 || codigo.Length == 48)
            {
                return EnumTipoCodigo.LinhaDigitavel;
            }
            else
            {
                return EnumTipoCodigo.TamanhoIncorreto;
            }
        }

        public static string IdentificarTipoBoleto(string codigo)
        {
            codigo = codigo.TratarCodigo();

            if (Convert.ToInt64(codigo[33..]) == 0 || Convert.ToInt64(codigo.Substring(5, 14)) == 0)
            {
                return "CARTAO_DE_CREDITO";
            }
            else if (codigo[..1] == "8") // significa codigo.Substring(0, 1)
            {
                if (codigo.Substring(1, 1) == "1")
                {
                    return "ARRECADACAO_PREFEITURA";
                }
                else if (codigo.Substring(1, 1) == "2")
                {
                    return "CONVENIO_SANEAMENTO";
                }
                else if (codigo.Substring(1, 1) == "3")
                {
                    return "CONVENIO_ENERGIA_ELETRICA_E_GAS";
                }
                else if (codigo.Substring(1, 1) == "4")
                {
                    return "CONVENIO_TELECOMUNICACOES";
                }
                else if (codigo.Substring(1, 1) == "5")
                {
                    return "ARRECADACAO_ORGAOS_GOVERNAMENTAIS";
                }
                else if (codigo.Substring(1, 1) == "6" || codigo.Substring(1, 1) == "9")
                {
                    return "OUTROS";
                }
                else if (codigo.Substring(1, 1) == "7")
                {
                    return "ARRECADACAO_TAXAS_DE_TRANSITO";
                }
            }
            return "BANCO";
        }

        public static EnumTipoBoleto IdentificarTipoDeBoleto(string codigo)
        {
            codigo = codigo.TratarCodigo();

            if (Convert.ToInt64(codigo[33..]) == 0 || Convert.ToInt64(codigo.Substring(5, 14)) == 0)
            {
                return EnumTipoBoleto.CartaoDeCredito;
            }
            else if (codigo[..1] == "8") // significa codigo.Substring(0, 1)
            {
                if (codigo.Substring(1, 1) == "1")
                {
                    return EnumTipoBoleto.ArrecadacaoPrefeitura;
                }
                else if (codigo.Substring(1, 1) == "2")
                {
                    return EnumTipoBoleto.ArrecadacaoSaneamento;
                }
                else if (codigo.Substring(1, 1) == "3")
                {
                    return EnumTipoBoleto.ArrecadacaoEnergiaEletricaOuGas;
                }
                else if (codigo.Substring(1, 1) == "4")
                {
                    return EnumTipoBoleto.ArrecadacaoTelecomunicacao;
                }
                else if (codigo.Substring(1, 1) == "5")
                {
                    return EnumTipoBoleto.ArrecadacaoOrgaosGovernamentais;
                }
                else if (codigo.Substring(1, 1) == "6" || codigo.Substring(1, 1) == "9")
                {
                    return EnumTipoBoleto.Outros;
                }
                else if (codigo.Substring(1, 1) == "7")
                {
                    return EnumTipoBoleto.ArrecadacaoTaxasDeTransito;
                }
            }
            return EnumTipoBoleto.Banco;
        }

        public static DateTime ObterData(string codigo)
        {

            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);
            var tipoDeCodigo = IdentificarTipoDeCodigo(codigo);

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                codigo = codigo.TratarCodigo();
            }
            else
            {
                codigo = codigo.FormatarBarrasCorretoArrecadacao();
            }

            var fatorData = string.Empty;
            var dataStart = new DateTime(1997, 10, 7);

            if (tipoDeCodigo == EnumTipoCodigo.CodigoDeBarras)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    fatorData = codigo.Substring(5, 4);
                }
                else
                {
                    fatorData = "0";
                }
            }
            else if (tipoDeCodigo == EnumTipoCodigo.LinhaDigitavel)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    fatorData = codigo.Substring(33, 4);
                }
                else
                {
                    fatorData = "0";
                }
            }

            var dataBoleto = dataStart.AddDays(Convert.ToInt32(fatorData));

            return dataBoleto;
        }

        public static DateOnly ObterData2(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);
            var tipoDeCodigo = IdentificarTipoDeCodigo(codigo);

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                codigo = codigo.TratarCodigo();
            }
            else
            {
                codigo = codigo.FormatarBarrasCorretoArrecadacao();
            }

            var fatorData = string.Empty;
            var dataStart = new DateOnly(1997, 10, 7);

            if (tipoDeCodigo == EnumTipoCodigo.CodigoDeBarras)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    fatorData = codigo.Substring(5, 4);
                }
                else
                {
                    fatorData = "0";
                }
            }
            else if (tipoDeCodigo == EnumTipoCodigo.LinhaDigitavel)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    fatorData = codigo.Substring(33, 4);
                }
                else
                {
                    fatorData = "0";
                }
            }

            var dataBoleto = dataStart.AddDays(Convert.ToInt32(fatorData));

            return dataBoleto;
        }

        public static decimal ObterValor(string codigo)
        {

            var retorno = 0m;

            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);
            var tipoDeCodigo = IdentificarTipoDeCodigo(codigo);

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                codigo = codigo.TratarCodigo();
            }
            else
            {
                codigo = codigo.FormatarBarrasCorretoArrecadacao();
            }


            var valorBoleto = string.Empty;
            var valorFinal = 0;

            if (tipoDeCodigo == EnumTipoCodigo.CodigoDeBarras)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    valorBoleto = codigo.Substring(9, 10);
                }
                else
                {
                    valorFinal = ObterValorArrecadacao(codigo);
                }
                valorFinal = Convert.ToInt32(valorBoleto);
                retorno = valorFinal / 100;

            }
            else if (tipoDeCodigo == EnumTipoCodigo.LinhaDigitavel)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    valorBoleto = codigo.Substring(37);
                    valorFinal = Convert.ToInt32(valorBoleto);
                }
                else
                {
                    valorFinal = ObterValorArrecadacao(codigo);
                }
                retorno = (valorFinal / 100m);

            }

            return retorno;
        }

        public static string CodigoDeBarras2LinhaDigitavel(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                codigo = codigo.TratarCodigo();

                var novaLinha = codigo[..4] + codigo.Substring(19, 25) + codigo.Substring(4, 1) + codigo.Substring(5, 14);

                var bloco1 = novaLinha[..9] + CalculaModulo10(novaLinha.Substring(0, 9)).ToString();
                var bloco2 = novaLinha.Substring(9, 10) + CalculaModulo10(novaLinha.Substring(9, 10)).ToString();
                var bloco3 = novaLinha.Substring(19, 10) + CalculaModulo10(novaLinha.Substring(19, 10)).ToString();
                var bloco4 = novaLinha[29..];

                return bloco1 + bloco2 + bloco3 + bloco4;

            }
            else
            {
                return codigo.FormatarBarrasCorretoArrecadacao();
            }


        }

        public static bool ValidarLinhaDigitavelBoletoArrecadacao(string linha)
        {
            if (linha.Length != 48)
            {
                return false;
            }

            var bloco1 = linha.Substring(0, 11);
            var bloco2 = linha.Substring(12, 11);
            var bloco3 = linha.Substring(24, 11);
            var bloco4 = linha.Substring(36, 11);
            var digito1 = linha.Substring(11, 1);
            var digito2 = linha.Substring(23, 1);
            var digito3 = linha.Substring(35, 1);
            var digito4 = linha.Substring(47, 1);

            var digitoVerificador1 = CalculaModulo10(bloco1).ToString();
            var digitoVerificador2 = CalculaModulo10(bloco2).ToString();
            var digitoVerificador3 = CalculaModulo10(bloco3).ToString();
            var digitoVerificador4 = CalculaModulo10(bloco4).ToString();

            if (digito1 != digitoVerificador1)
            {
                return false;
            }
            if (digito2 != digitoVerificador2)
            {
                return false;
            }
            if (digito3 != digitoVerificador3)
            {
                return false;
            }

            return digito4 == digitoVerificador4;

        }
        public static string LinhaDigitavel2CodigoDeBarras(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);

            var resultado = codigo.TratarCodigo();

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                codigo = codigo.TratarCodigo();
                resultado = codigo[..4] +
                    codigo.Substring(32, 1) +
                    codigo.Substring(33, 14) +
                    codigo.Substring(4, 5) +
                    codigo.Substring(10, 10) +
                    codigo.Substring(21, 10);
            }
            else
            {
                resultado = ObterCodigodeBarrasCorreto(codigo);
            }

            return resultado;

        }

        private static string ObterCodigodeBarrasCorreto(string codigoDeBarras)
        {
            if (codigoDeBarras.Length == 52)
            {
                codigoDeBarras = codigoDeBarras.Replace("-", "").Replace(" ", "");
                var codigo1 = codigoDeBarras.Substring(0, 11);
                var codigo2 = codigoDeBarras.Substring(12, 11);
                var codigo3 = codigoDeBarras.Substring(24, 11);
                var codigo4 = codigoDeBarras.Substring(36, 11);
                return $"{codigo1}{codigo2}{codigo3}{codigo4}";

            }
            if (codigoDeBarras.Length == 48)
            {
                codigoDeBarras = codigoDeBarras.Replace("-", "").Replace(" ", "");
                var codigo1 = codigoDeBarras.Substring(0, 11);
                var codigo2 = codigoDeBarras.Substring(12, 11);
                var codigo3 = codigoDeBarras.Substring(24, 11);
                var codigo4 = codigoDeBarras.Substring(36, 11);
                return $"{codigo1}{codigo2}{codigo3}{codigo4}";

            }
            else
            {
                var codigos = codigoDeBarras.Split(' ');
                var codigo1 = codigos[0].Split('-');
                var codigo2 = codigos[1].Split('-');
                var codigo3 = codigos[2].Split('-');
                var codigo4 = codigos[3].Split('-');
                return $"{codigo1[0]}{codigo2[0]}{codigo3[0]}{codigo4[0]}";
            }
        }
        private static int CalculaModulo10(string numero)
        {
            var soma = 0;
            var peso = 2;
            var contador = numero.Trim().Length - 1;
            while (contador >= 0)
            {
                var multiplicacao = (int.Parse(numero.Substring(contador, 1)) * peso);
                if (multiplicacao >= 10) { multiplicacao = 1 + (multiplicacao - 10); }
                soma += multiplicacao;
                peso = peso == 2 ? 1 : 2;
                contador--;
            }
            var digito = 10 - (soma % 10);
            if (digito == 10) digito = 0;
            return digito;
        }


        private static int ObterValorArrecadacao(string codigo)
        {
            var retorno = 0;

            var tipoCodigo = IdentificarTipoDeCodigo(codigo);
            var isValorEfetivo = IdentificarReferencia(codigo);

            var valorBoleto = "";

            if (isValorEfetivo)
            {
                valorBoleto = codigo.Substring(4, 11);
            }
            else
            {
                valorBoleto = "0";
            }

            retorno = Convert.ToInt32(valorBoleto);

            return retorno;
        }

        private static bool IdentificarReferencia(string codigo)
        {

            switch (codigo.Substring(2, 1))
            {
                case "6":
                    return true;
                case "7":
                    return false;
                case "8":
                    return true;
                case "9":
                default:
                    return false;
            }
        }

        private static string TratarCodigo(this string codigo)
        {
            codigo = codigo.Replace(" ", "");
            codigo = codigo.Replace(".", "");
            codigo = codigo.Replace("-", "");

            return codigo;
        }

        private static string FormatarBarrasCorretoArrecadacao(this string codigo)
        {
            if (codigo.Length == 52)
            {
                codigo = codigo.Replace("-", "").Replace(" ", "");
                var codigo1 = codigo.Substring(0, 11);
                var codigo2 = codigo.Substring(12, 11);
                var codigo3 = codigo.Substring(24, 11);
                var codigo4 = codigo.Substring(36, 11);
                return $"{codigo1}{codigo2}{codigo3}{codigo4}";

            }
            else
            {
                var codigos = codigo.Split(' ');
                var codigo1 = codigos[0].Split('-');
                var codigo2 = codigos[1].Split('-');
                var codigo3 = codigos[2].Split('-');
                var codigo4 = codigos[3].Split('-');
                return $"{codigo1[0]}{codigo2[0]}{codigo3[0]}{codigo4[0]}";
            }
        }

    }

}
