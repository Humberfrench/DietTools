namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Formato do código informado, definido pelo tamanho normalizado e pelo
/// primeiro dígito (SPEC §3).
/// </summary>
public enum TipoCodigoBoleto
{
    /// <summary>Tamanho ou primeiro dígito fora dos quatro formatos suportados.</summary>
    NaoSuportado = 0,

    /// <summary>44 dígitos, primeiro dígito diferente de <c>8</c> (ficha de compensação).</summary>
    CodigoBarrasBancario = 1,

    /// <summary>47 dígitos (ficha de compensação).</summary>
    LinhaDigitavelBancaria = 2,

    /// <summary>44 dígitos iniciados em <c>8</c> (arrecadação/convênios).</summary>
    CodigoBarrasArrecadacao = 3,

    /// <summary>48 dígitos iniciados em <c>8</c> (arrecadação/convênios).</summary>
    LinhaDigitavelArrecadacao = 4
}
