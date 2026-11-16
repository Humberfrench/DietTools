namespace Dietcode.Core.Lib.Boletos;

/// <summary>Códigos estáveis de <see cref="BoletoValidacaoError.Code"/>.</summary>
public static class BoletoValidacaoErrorCodes
{
    /// <summary>Código nulo, vazio ou só com separadores.</summary>
    public const string CodigoVazio = "CODIGO_VAZIO";
    /// <summary>Há caractere que não é dígito nem separador (espaço, ponto, hífen).</summary>
    public const string CodigoInvalido = "CODIGO_INVALIDO";
    /// <summary>Quantidade de dígitos diferente de 44, 47 e 48.</summary>
    public const string TamanhoInvalido = "TAMANHO_INVALIDO";
    /// <summary>Tamanho aceito, mas formato não (p.ex. 48 dígitos que não começam com 8).</summary>
    public const string TipoNaoSuportado = "TIPO_NAO_SUPORTADO";
    /// <summary>Moeda diferente de 9 (Real).</summary>
    public const string MoedaInvalida = "MOEDA_INVALIDA";
    /// <summary>DV geral do código de barras bancário inválido.</summary>
    public const string DvInvalido = "DV_INVALIDO";
    /// <summary>DV do campo 1 da linha digitável bancária inválido.</summary>
    public const string Campo1Invalido = "CAMPO_1_INVALIDO";
    /// <summary>DV do campo 2 da linha digitável bancária inválido.</summary>
    public const string Campo2Invalido = "CAMPO_2_INVALIDO";
    /// <summary>DV do campo 3 da linha digitável bancária inválido.</summary>
    public const string Campo3Invalido = "CAMPO_3_INVALIDO";
    /// <summary>DV geral (posição 4) da arrecadação inválido.</summary>
    public const string ArrecadacaoDvInvalido = "ARRECADACAO_DV_INVALIDO";
    /// <summary>DV do bloco 1 da linha digitável de arrecadação inválido.</summary>
    public const string ArrecadacaoCampo1Invalido = "ARRECADACAO_CAMPO_1_INVALIDO";
    /// <summary>DV do bloco 2 da linha digitável de arrecadação inválido.</summary>
    public const string ArrecadacaoCampo2Invalido = "ARRECADACAO_CAMPO_2_INVALIDO";
    /// <summary>DV do bloco 3 da linha digitável de arrecadação inválido.</summary>
    public const string ArrecadacaoCampo3Invalido = "ARRECADACAO_CAMPO_3_INVALIDO";
    /// <summary>DV do bloco 4 da linha digitável de arrecadação inválido.</summary>
    public const string ArrecadacaoCampo4Invalido = "ARRECADACAO_CAMPO_4_INVALIDO";
    /// <summary>Terceiro dígito da arrecadação diferente de 6, 7, 8 ou 9.</summary>
    public const string ArrecadacaoIndicadorInvalido = "ARRECADACAO_INDICADOR_INVALIDO";
    /// <summary>Segmento fora do mapa conhecido. Não bloqueia.</summary>
    public const string SegmentoDesconhecido = "SEGMENTO_DESCONHECIDO";
    /// <summary>Fator de vencimento entre 0001 e 0999.</summary>
    public const string FatorVencimentoInvalido = "FATOR_VENCIMENTO_INVALIDO";
    /// <summary>O código não define o vencimento. Bloqueia, ou só informa quando vale a regra D+1.</summary>
    public const string VencimentoIndeterminado = "VENCIMENTO_INDETERMINADO";
    /// <summary>Vencimento anterior a hoje. Bloqueia conforme a opção BloquearVencido.</summary>
    public const string BoletoVencido = "BOLETO_VENCIDO";
    /// <summary>Valor zerado ou por referência. Não bloqueia.</summary>
    public const string ValorNaoInformado = "VALOR_NAO_INFORMADO";
}

/// <summary>
/// Problema encontrado na validação. <see cref="Bloqueante"/> distingue o que
/// invalida o boleto (DV, tamanho…) do que é só informação para o consumidor
/// (valor não informado, vencimento estimado…).
/// </summary>
/// <param name="Code">Código estável, ver <see cref="BoletoValidacaoErrorCodes"/>.</param>
/// <param name="Message">Mensagem em português.</param>
/// <param name="Bloqueante">Quando <c>true</c>, <see cref="BoletoValidacaoResult.IsValid"/> fica <c>false</c>.</param>
public sealed record BoletoValidacaoError(string Code, string Message, bool Bloqueante = true);
