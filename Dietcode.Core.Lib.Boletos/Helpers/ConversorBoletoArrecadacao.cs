namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Conversões 44↔48 da arrecadação: a linha digitável são quatro blocos de 11
/// dígitos do código de barras, cada um seguido do seu DV. Substitui
/// <c>ObterCodigodeBarrasCorreto</c> e <c>FormatarBarrasCorretoArrecadacao</c>, que
/// dependiam de heurísticas de separadores.
/// </summary>
internal static class ConversorBoletoArrecadacao
{
    private const int TamanhoBloco = 11;

    /// <summary>
    /// Converte 44 dígitos em linha digitável de 48. O módulo dos DVs vem do indicador
    /// da posição 3 do código de barras.
    /// </summary>
    /// <returns>A linha, ou <c>null</c> se o indicador não for 6, 7, 8 ou 9.</returns>
    public static string? ParaLinhaDigitavel(string codigoBarras)
    {
        if (codigoBarras.Length != 44)
            throw new ArgumentException("O código de barras de arrecadação deve ter 44 dígitos.", nameof(codigoBarras));

        var indicador = codigoBarras[2];
        var linha = new System.Text.StringBuilder(48);

        for (var bloco = 0; bloco < 4; bloco++)
        {
            var digitos = codigoBarras.Substring(bloco * TamanhoBloco, TamanhoBloco);
            var dv = DigitoVerificadorHelper.DigitoArrecadacao(indicador, digitos);
            if (dv is null)
                return null;

            linha.Append(digitos).Append(dv.Value);
        }

        return linha.ToString();
    }

    /// <summary>Converte a linha digitável de 48 dígitos em código de barras de 44, descartando os 4 DVs.</summary>
    public static string ParaCodigoBarras(string linhaDigitavel)
    {
        if (linhaDigitavel.Length != 48)
            throw new ArgumentException("A linha digitável de arrecadação deve ter 48 dígitos.", nameof(linhaDigitavel));

        return linhaDigitavel.Substring(0, 11)
             + linhaDigitavel.Substring(12, 11)
             + linhaDigitavel.Substring(24, 11)
             + linhaDigitavel.Substring(36, 11);
    }
}
