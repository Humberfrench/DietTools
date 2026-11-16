namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Conversões 44↔47 do boleto bancário. Reaproveitam os índices de
/// <c>Validator.CodigoDeBarras2LinhaDigitavel</c> e <c>LinhaDigitavel2CodigoDeBarras</c>,
/// agora com tamanho exato (o legado devolvia a própria entrada quando curta).
/// Entradas só com dígitos; o chamador valida antes.
/// </summary>
/// <remarks>
/// Código de barras (0-based): [0..2] banco, [3] moeda, [4] DV geral, [5..8] fator,
/// [9..18] valor, [19..43] campo livre. Linha digitável (0-based): [0..8] banco+moeda+5 do
/// campo livre, [9] DV1, [10..19] campo livre, [20] DV2, [21..30] campo livre, [31] DV3,
/// [32] DV geral, [33..36] fator, [37..46] valor.
/// </remarks>
internal static class ConversorBoletoBancario
{
    /// <summary>Converte 44 dígitos em linha digitável de 47, calculando os três DVs Módulo 10.</summary>
    public static string ParaLinhaDigitavel(string codigoBarras)
    {
        if (codigoBarras.Length != 44)
            throw new ArgumentException("O código de barras bancário deve ter 44 dígitos.", nameof(codigoBarras));

        var campo1 = codigoBarras[..4] + codigoBarras.Substring(19, 5);
        var campo2 = codigoBarras.Substring(24, 10);
        var campo3 = codigoBarras.Substring(34, 10);

        return campo1 + DigitoVerificadorHelper.Modulo10(campo1)
             + campo2 + DigitoVerificadorHelper.Modulo10(campo2)
             + campo3 + DigitoVerificadorHelper.Modulo10(campo3)
             + codigoBarras.Substring(4, 1)     // DV geral
             + codigoBarras.Substring(5, 14);   // fator + valor
    }

    /// <summary>Converte a linha digitável de 47 dígitos em código de barras de 44, descartando os DVs dos campos.</summary>
    public static string ParaCodigoBarras(string linhaDigitavel)
    {
        if (linhaDigitavel.Length != 47)
            throw new ArgumentException("A linha digitável bancária deve ter 47 dígitos.", nameof(linhaDigitavel));

        return linhaDigitavel[..4]                   // banco + moeda
             + linhaDigitavel.Substring(32, 1)       // DV geral
             + linhaDigitavel.Substring(33, 14)      // fator + valor
             + linhaDigitavel.Substring(4, 5)        // campo livre (1/3)
             + linhaDigitavel.Substring(10, 10)      // campo livre (2/3)
             + linhaDigitavel.Substring(21, 10);     // campo livre (3/3)
    }
}
