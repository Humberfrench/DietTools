namespace Dietcode.Core.Lib.Boletos;

/// <summary>Ponto único de entrada da engine de validação de boletos.</summary>
public interface IBoletoValidator
{
    /// <summary>
    /// Normaliza, classifica e valida o código, devolvendo valor, vencimento,
    /// multa e desconto. Nunca lança por entrada inválida: os problemas voltam em
    /// <see cref="BoletoValidacaoResult.Errors"/>.
    /// </summary>
    /// <param name="codigo">Código de barras (44), linha digitável bancária (47) ou de arrecadação (48), com ou sem separadores.</param>
    /// <param name="options">Políticas desta chamada; <c>null</c> usa as configuradas no validador.</param>
    BoletoValidacaoResult Validar(string? codigo, BoletoValidacaoOptions? options = null);

    /// <summary>Identifica o formato sem validar dígitos verificadores.</summary>
    TipoCodigoBoleto IdentificarTipo(string? codigo);
}
