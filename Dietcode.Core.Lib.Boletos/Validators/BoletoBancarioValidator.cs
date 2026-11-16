namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Regras do boleto bancário (SPEC §4): moeda, DV geral Módulo 11 e, na linha
/// digitável, os três DVs Módulo 10. O código do banco <b>não</b> é validado contra
/// catálogo (decisão de 16/11/2026): o fluxo segue para extrair valor e vencimento.
/// </summary>
internal sealed class BoletoBancarioValidator : IFormatoBoletoValidator
{
    private const char MoedaReal = '9';

    /// <inheritdoc />
    public FormatoBoletoResultado ValidarCodigoBarras(string codigoBarras)
    {
        var resultado = new FormatoBoletoResultado();
        AnalisarCodigoBarras(resultado, codigoBarras);
        resultado.LinhaDigitavel = ConversorBoletoBancario.ParaLinhaDigitavel(codigoBarras);
        return resultado;
    }

    /// <inheritdoc />
    public FormatoBoletoResultado ValidarLinhaDigitavel(string linhaDigitavel)
    {
        var resultado = new FormatoBoletoResultado { LinhaDigitavel = linhaDigitavel };

        // Campos 1, 2 e 3: bloco (índices 0-based) + DV Módulo 10.
        ValidarCampo(resultado, linhaDigitavel, 0, 9, BoletoValidacaoErrorCodes.Campo1Invalido, "1");
        ValidarCampo(resultado, linhaDigitavel, 10, 20, BoletoValidacaoErrorCodes.Campo2Invalido, "2");
        ValidarCampo(resultado, linhaDigitavel, 21, 31, BoletoValidacaoErrorCodes.Campo3Invalido, "3");

        var codigoBarras = ConversorBoletoBancario.ParaCodigoBarras(linhaDigitavel);
        AnalisarCodigoBarras(resultado, codigoBarras);

        return resultado;
    }

    private static void ValidarCampo(
        FormatoBoletoResultado resultado, string linha, int inicio, int indiceDv, string codigoErro, string numero)
    {
        var bloco = linha.Substring(inicio, indiceDv - inicio);
        var esperado = DigitoVerificadorHelper.Modulo10(bloco);

        if (linha[indiceDv] - '0' != esperado)
            resultado.Errors.Add(new BoletoValidacaoError(codigoErro, $"O dígito verificador do campo {numero} da linha digitável é inválido."));
    }

    private static void AnalisarCodigoBarras(FormatoBoletoResultado resultado, string codigoBarras)
    {
        resultado.CodigoBarras = codigoBarras;
        resultado.BancoCodigo = codigoBarras[..3];

        if (codigoBarras[3] != MoedaReal)
            resultado.Errors.Add(new BoletoValidacaoError(BoletoValidacaoErrorCodes.MoedaInvalida, "Moeda não suportada: apenas o Real (9) é aceito."));

        // DV geral (posição 5, índice 4) calculado sobre os outros 43 dígitos.
        var semDv = codigoBarras[..4] + codigoBarras[5..];
        if (codigoBarras[4] - '0' != DigitoVerificadorHelper.Modulo11Bancario(semDv))
            resultado.Errors.Add(new BoletoValidacaoError(BoletoValidacaoErrorCodes.DvInvalido, "O dígito verificador geral do código de barras é inválido."));

        resultado.Fator = int.Parse(codigoBarras.AsSpan(5, 4));
        resultado.ValorCentavos = long.Parse(codigoBarras.AsSpan(9, 10));
        resultado.PossivelBoletoCartao = resultado.Fator == 0 && resultado.ValorCentavos == 0;
    }
}
