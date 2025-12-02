namespace Dietcode.Core.Lib
{
    public static class BoletoValidator
    {
        #region Constantes de tipos de código

        private const string TIPO_CODIGO_DE_BARRAS = "CODIGO_DE_BARRAS";
        private const string TIPO_LINHA_DIGITAVEL = "LINHA_DIGITAVEL";
        private const string TIPO_TAMANHO_INCORRETO = "TAMANHO_INCORRETO";

        #endregion

        #region Constantes de tipos de boleto (string)

        private const string BOLETO_BANCO = "BANCO";
        private const string BOLETO_CARTAO_DE_CREDITO = "CARTAO_DE_CREDITO";

        private const string BOLETO_ARRECADACAO_PREFEITURA = "ARRECADACAO_PREFEITURA";
        private const string BOLETO_CONVENIO_SANEAMENTO = "CONVENIO_SANEAMENTO";
        private const string BOLETO_CONVENIO_ENERGIA_ELETRICA_GAS = "CONVENIO_ENERGIA_ELETRICA_E_GAS";
        private const string BOLETO_CONVENIO_TELECOMUNICACOES = "CONVENIO_TELECOMUNICACOES";
        private const string BOLETO_ARRECADACAO_ORGAOS_GOVERNAMENTAIS = "ARRECADACAO_ORGAOS_GOVERNAMENTAIS";
        private const string BOLETO_ARRECADACAO_TAXAS_DE_TRANSITO = "ARRECADACAO_TAXAS_DE_TRANSITO";
        private const string BOLETO_OUTROS = "OUTROS";

        #endregion

        #region Identificação de tipo de código

        public static string IdentificarTipoCodigo(string codigo)
        {
            var tipo = IdentificarTipoDeCodigo(codigo);

            return tipo switch
            {
                EnumTipoCodigo.CodigoDeBarras => TIPO_CODIGO_DE_BARRAS,
                EnumTipoCodigo.LinhaDigitavel => TIPO_LINHA_DIGITAVEL,
                _ => TIPO_TAMANHO_INCORRETO
            };
        }

        public static EnumTipoCodigo IdentificarTipoDeCodigo(string codigo)
        {
            codigo = codigo.TratarCodigo();

            return codigo.Length switch
            {
                44 => EnumTipoCodigo.CodigoDeBarras,
                46 or 47 or 48 => EnumTipoCodigo.LinhaDigitavel,
                _ => EnumTipoCodigo.TamanhoIncorreto
            };
        }

        #endregion

        #region Identificação de tipo de boleto

        public static string IdentificarTipoBoleto(string codigo)
        {
            var tipo = IdentificarTipoDeBoleto(codigo);

            return tipo switch
            {
                EnumTipoBoleto.CartaoDeCredito => BOLETO_CARTAO_DE_CREDITO,
                EnumTipoBoleto.ArrecadacaoPrefeitura => BOLETO_ARRECADACAO_PREFEITURA,
                EnumTipoBoleto.ArrecadacaoSaneamento => BOLETO_CONVENIO_SANEAMENTO,
                EnumTipoBoleto.ArrecadacaoEnergiaEletricaOuGas => BOLETO_CONVENIO_ENERGIA_ELETRICA_GAS,
                EnumTipoBoleto.ArrecadacaoTelecomunicacao => BOLETO_CONVENIO_TELECOMUNICACOES,
                EnumTipoBoleto.ArrecadacaoOrgaosGovernamentais => BOLETO_ARRECADACAO_ORGAOS_GOVERNAMENTAIS,
                EnumTipoBoleto.Outros => BOLETO_OUTROS,
                EnumTipoBoleto.ArrecadacaoTaxasDeTransito => BOLETO_ARRECADACAO_TAXAS_DE_TRANSITO,
                _ => BOLETO_BANCO
            };
        }

        public static EnumTipoBoleto IdentificarTipoDeBoleto(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return EnumTipoBoleto.Banco;

            codigo = codigo.TratarCodigo();

            // Protege contra códigos muito curtos para os Substring abaixo
            var len = codigo.Length;

            // Regra de cartão de crédito:
            // - trecho de posição 33 até o fim = 0  OU
            // - trecho pos (5, 14) = 0
            if (len >= 34 && long.TryParse(codigo[33..], out var tail) && tail == 0
                || len >= 19 && long.TryParse(codigo.Substring(5, 14), out var meio) && meio == 0)
            {
                return EnumTipoBoleto.CartaoDeCredito;
            }

            // Boletos de arrecadação começam com '8'
            if (len > 1 && codigo[0] == '8')
            {
                var tipoSegmento = codigo.Substring(1, 1);

                return tipoSegmento switch
                {
                    "1" => EnumTipoBoleto.ArrecadacaoPrefeitura,
                    "2" => EnumTipoBoleto.ArrecadacaoSaneamento,
                    "3" => EnumTipoBoleto.ArrecadacaoEnergiaEletricaOuGas,
                    "4" => EnumTipoBoleto.ArrecadacaoTelecomunicacao,
                    "5" => EnumTipoBoleto.ArrecadacaoOrgaosGovernamentais,
                    "6" or "9" => EnumTipoBoleto.Outros,
                    "7" => EnumTipoBoleto.ArrecadacaoTaxasDeTransito,
                    _ => EnumTipoBoleto.Banco
                };
            }

            return EnumTipoBoleto.Banco;
        }

        #endregion

        #region Data (DateTime / DateOnly)

        public static DateTime ObterData(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);
            var tipoDeCodigo = IdentificarTipoDeCodigo(codigo);

            codigo = NormalizarCodigoParaDataEValor(codigo, tipoDeBoleto);

            var fatorData = ObterFatorData(codigo, tipoDeBoleto, tipoDeCodigo);
            var dataStart = new DateTime(1997, 10, 7);

            if (!int.TryParse(fatorData, out var fatorInt))
                fatorInt = 0;

            return dataStart.AddDays(fatorInt);
        }

        public static DateOnly ObterData2(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);
            var tipoDeCodigo = IdentificarTipoDeCodigo(codigo);

            codigo = NormalizarCodigoParaDataEValor(codigo, tipoDeBoleto);

            var fatorData = ObterFatorData(codigo, tipoDeBoleto, tipoDeCodigo);
            var dataStart = new DateOnly(1997, 10, 7);

            if (!int.TryParse(fatorData, out var fatorInt))
                fatorInt = 0;

            return dataStart.AddDays(fatorInt);
        }

        private static string ObterFatorData(string codigo, EnumTipoBoleto tipoDeBoleto, EnumTipoCodigo tipoDeCodigo)
        {
            // Apenas boletos bancários / cartão usam fator data
            var ehBancoOuCartao = tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito;

            if (!ehBancoOuCartao)
                return "0";

            var len = codigo.Length;

            if (tipoDeCodigo == EnumTipoCodigo.CodigoDeBarras)
            {
                // fator na posição 5 (0-based) com 4 dígitos
                return len >= 9 ? codigo.Substring(5, 4) : "0";
            }

            if (tipoDeCodigo == EnumTipoCodigo.LinhaDigitavel)
            {
                // fator na posição 33 com 4 dígitos (linha digitável)
                return len >= 37 ? codigo.Substring(33, 4) : "0";
            }

            return "0";
        }

        #endregion

        #region Valor

        public static decimal ObterValor(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);
            var tipoDeCodigo = IdentificarTipoDeCodigo(codigo);

            codigo = NormalizarCodigoParaDataEValor(codigo, tipoDeBoleto);

            var valorFinal = 0;

            if (tipoDeCodigo == EnumTipoCodigo.CodigoDeBarras)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    // Posição 9 com 10 dígitos
                    if (codigo.Length >= 19)
                    {
                        var valorBoleto = codigo.Substring(9, 10);
                        if (!int.TryParse(valorBoleto, out valorFinal))
                            valorFinal = 0;
                    }
                }
                else
                {
                    valorFinal = ObterValorArrecadacao(codigo);
                }

                return valorFinal / 100m;
            }

            if (tipoDeCodigo == EnumTipoCodigo.LinhaDigitavel)
            {
                if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
                {
                    // Da posição 37 até o fim
                    if (codigo.Length >= 38)
                    {
                        var valorBoleto = codigo.Substring(37);
                        if (!int.TryParse(valorBoleto, out valorFinal))
                            valorFinal = 0;
                    }
                }
                else
                {
                    valorFinal = ObterValorArrecadacao(codigo);
                }

                return valorFinal / 100m;
            }

            return 0m;
        }

        #endregion

        #region Conversão Código de Barras ↔ Linha Digitável

        public static string CodigoDeBarras2LinhaDigitavel(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                codigo = codigo.TratarCodigo();

                if (codigo.Length < 44)
                    return codigo; // não tenta formatar se inválido

                // Monta nova linha
                var novaLinha = codigo[..4] + // banco + moeda
                                codigo.Substring(19, 25) + // campo livre
                                codigo.Substring(4, 1) +   // DV geral
                                codigo.Substring(5, 14);   // fator + valor

                var bloco1 = novaLinha[..9];
                var bloco2 = novaLinha.Substring(9, 10);
                var bloco3 = novaLinha.Substring(19, 10);
                var bloco4 = novaLinha[29..];

                var bloco1Completo = bloco1 + CalculaModulo10(bloco1).ToString();
                var bloco2Completo = bloco2 + CalculaModulo10(bloco2).ToString();
                var bloco3Completo = bloco3 + CalculaModulo10(bloco3).ToString();

                return bloco1Completo + bloco2Completo + bloco3Completo + bloco4;
            }

            // Arrecadação: usa formatação específica
            return codigo.FormatarBarrasCorretoArrecadacao();
        }

        public static string LinhaDigitavel2CodigoDeBarras(string codigo)
        {
            var tipoDeBoleto = IdentificarTipoDeBoleto(codigo);

            var resultado = codigo.TratarCodigo();

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                codigo = codigo.TratarCodigo();

                if (codigo.Length < 47) // linha digitável bancária padrão
                    return codigo;

                // Monta código de barras bancário
                resultado = codigo[..4] +
                            codigo.Substring(32, 1) +    // DV geral
                            codigo.Substring(33, 14) +   // fator + valor
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

        #endregion

        #region Validação linha digitável arrecadação

        public static bool ValidarLinhaDigitavelBoletoArrecadacao(string linha)
        {
            if (string.IsNullOrWhiteSpace(linha))
                return false;

            linha = linha.TratarCodigo();

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

            if (digito1 != digitoVerificador1) return false;
            if (digito2 != digitoVerificador2) return false;
            if (digito3 != digitoVerificador3) return false;

            return digito4 == digitoVerificador4;
        }

        #endregion

        #region Helpers privados

        private static string ObterCodigodeBarrasCorreto(string codigoDeBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoDeBarras))
                return string.Empty;

            codigoDeBarras = codigoDeBarras.Replace("-", "").Replace(" ", "");

            if (codigoDeBarras.Length == 52 || codigoDeBarras.Length == 48)
            {
                var codigo1 = codigoDeBarras.Substring(0, 11);
                var codigo2 = codigoDeBarras.Substring(12, 11);
                var codigo3 = codigoDeBarras.Substring(24, 11);
                var codigo4 = codigoDeBarras.Substring(36, 11);
                return $"{codigo1}{codigo2}{codigo3}{codigo4}";
            }

            // Formato "XXXX-? XXXX-? XXXX-? XXXX-?"
            var codigos = codigoDeBarras.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (codigos.Length != 4) return codigoDeBarras;

            var c1 = codigos[0].Split('-');
            var c2 = codigos[1].Split('-');
            var c3 = codigos[2].Split('-');
            var c4 = codigos[3].Split('-');

            return $"{c1[0]}{c2[0]}{c3[0]}{c4[0]}";
        }

        private static int CalculaModulo10(string numero)
        {
            numero = numero?.Trim() ?? string.Empty;

            var soma = 0;
            var peso = 2;
            var contador = numero.Length - 1;

            while (contador >= 0)
            {
                if (!int.TryParse(numero.Substring(contador, 1), out var digito))
                {
                    contador--;
                    continue;
                }

                var multiplicacao = digito * peso;
                if (multiplicacao >= 10)
                {
                    multiplicacao = 1 + (multiplicacao - 10);
                }

                soma += multiplicacao;
                peso = peso == 2 ? 1 : 2;
                contador--;
            }

            var dv = 10 - (soma % 10);
            if (dv == 10) dv = 0;
            return dv;
        }

        private static int ObterValorArrecadacao(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return 0;

            codigo = codigo.TratarCodigo();

            var isValorEfetivo = IdentificarReferencia(codigo);

            if (!isValorEfetivo)
                return 0;

            // Valor começa na posição 4 com 11 dígitos (para arrecadação)
            if (codigo.Length < 15)
                return 0;

            var valorBoleto = codigo.Substring(4, 11);

            return int.TryParse(valorBoleto, out var valor) ? valor : 0;
        }

        private static bool IdentificarReferencia(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length < 3)
                return false;

            return codigo.Substring(2, 1) switch
            {
                "6" => true,
                "7" => false,
                "8" => true,
                "9" => false,
                _ => false
            };
        }

        private static string TratarCodigo(this string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return string.Empty;

            return codigo
                .Replace(" ", "")
                .Replace(".", "")
                .Replace("-", "");
        }

        private static string FormatarBarrasCorretoArrecadacao(this string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return string.Empty;

            codigo = codigo.Replace("-", "").Replace(" ", "");

            if (codigo.Length == 52)
            {
                var codigo1 = codigo.Substring(0, 11);
                var codigo2 = codigo.Substring(12, 11);
                var codigo3 = codigo.Substring(24, 11);
                var codigo4 = codigo.Substring(36, 11);
                return $"{codigo1}{codigo2}{codigo3}{codigo4}";
            }

            // Formato "XXXX-? XXXX-? XXXX-? XXXX-?"
            var codigos = codigo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (codigos.Length != 4) return codigo;

            var c1 = codigos[0].Split('-');
            var c2 = codigos[1].Split('-');
            var c3 = codigos[2].Split('-');
            var c4 = codigos[3].Split('-');

            return $"{c1[0]}{c2[0]}{c3[0]}{c4[0]}";
        }

        private static string NormalizarCodigoParaDataEValor(string codigo, EnumTipoBoleto tipoDeBoleto)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return string.Empty;

            if (tipoDeBoleto == EnumTipoBoleto.Banco || tipoDeBoleto == EnumTipoBoleto.CartaoDeCredito)
            {
                return codigo.TratarCodigo();
            }

            return codigo.FormatarBarrasCorretoArrecadacao();
        }

        #endregion
    }
}
