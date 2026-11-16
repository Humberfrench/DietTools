namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Validador de um formato (bancário ou arrecadação). Recebe código já
/// normalizado (só dígitos) e com o tamanho certo para o método chamado.
/// </summary>
internal interface IFormatoBoletoValidator
{
    /// <summary>Valida um código de barras de 44 dígitos.</summary>
    FormatoBoletoResultado ValidarCodigoBarras(string codigoBarras, BoletoValidacaoOptions options);

    /// <summary>Valida uma linha digitável (47 dígitos bancária, 48 de arrecadação).</summary>
    FormatoBoletoResultado ValidarLinhaDigitavel(string linhaDigitavel, BoletoValidacaoOptions options);
}
