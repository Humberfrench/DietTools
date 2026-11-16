namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Normalização única do código (SPEC §3.1). Evolução de
/// <c>Validator.TratarCodigo</c>: aquele só removia separadores e deixava passar
/// qualquer outro caractere; aqui letras e símbolos são rejeitados.
/// </summary>
internal static class CodigoBoletoNormalizer
{
    /// <summary>
    /// Remove espaço, ponto e hífen (e espaços/quebras de linha nas pontas, comuns
    /// ao colar) e exige que sobrem apenas dígitos <c>0</c>–<c>9</c>. Mantém zeros
    /// à esquerda e nunca converte o código em número.
    /// </summary>
    /// <param name="codigo">Texto informado.</param>
    /// <param name="normalizado">Somente dígitos quando retorna <c>null</c>; vazio caso contrário.</param>
    /// <returns><c>null</c> se normalizou; senão o erro (<c>CODIGO_VAZIO</c> ou <c>CODIGO_INVALIDO</c>).</returns>
    public static BoletoValidacaoError? Normalizar(string? codigo, out string normalizado)
    {
        normalizado = string.Empty;

        if (string.IsNullOrWhiteSpace(codigo))
            return new BoletoValidacaoError(BoletoValidacaoErrorCodes.CodigoVazio, "O código do boleto é obrigatório.");

        var texto = codigo.Trim();
        var buffer = new char[texto.Length];
        var tamanho = 0;

        foreach (var c in texto)
        {
            if (c is ' ' or '.' or '-')
                continue;

            if (c is < '0' or > '9')
                return new BoletoValidacaoError(
                    BoletoValidacaoErrorCodes.CodigoInvalido,
                    "O código do boleto deve conter apenas números (espaço, ponto e hífen são aceitos como separadores).");

            buffer[tamanho++] = c;
        }

        if (tamanho == 0)
            return new BoletoValidacaoError(BoletoValidacaoErrorCodes.CodigoVazio, "O código do boleto é obrigatório.");

        normalizado = new string(buffer, 0, tamanho);
        return null;
    }

    /// <summary>
    /// Formato pelo tamanho e primeiro dígito de um código já normalizado (SPEC §3).
    /// </summary>
    public static TipoCodigoBoleto Classificar(string normalizado)
        => normalizado.Length switch
        {
            44 => normalizado[0] == '8' ? TipoCodigoBoleto.CodigoBarrasArrecadacao : TipoCodigoBoleto.CodigoBarrasBancario,
            47 => TipoCodigoBoleto.LinhaDigitavelBancaria,
            48 when normalizado[0] == '8' => TipoCodigoBoleto.LinhaDigitavelArrecadacao,
            _ => TipoCodigoBoleto.NaoSuportado
        };
}
