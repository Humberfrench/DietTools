namespace Dietcode.Core.Lib.Boletos;

/// <summary>Resultado da validação de um código de boleto.</summary>
public sealed class BoletoValidacaoResult
{
    /// <summary><c>true</c> quando não há nenhum erro bloqueante em <see cref="Errors"/>.</summary>
    public bool IsValid { get; init; }

    /// <summary>Formato identificado.</summary>
    public TipoCodigoBoleto TipoCodigo { get; init; }

    /// <summary>Texto recebido, sem alteração.</summary>
    public string CodigoOriginal { get; init; } = string.Empty;

    /// <summary>Somente dígitos, sem separadores e preservando zeros à esquerda.</summary>
    public string CodigoNormalizado { get; init; } = string.Empty;

    /// <summary>Código de barras (44 dígitos), quando foi possível obtê-lo.</summary>
    public string? CodigoBarras { get; init; }

    /// <summary>Linha digitável (47 ou 48 dígitos), quando foi possível obtê-la.</summary>
    public string? LinhaDigitavel { get; init; }

    /// <summary>Código da instituição (bancário). Não é validado contra catálogo.</summary>
    public string? BancoCodigo { get; init; }

    /// <summary>Segmento, só para arrecadação.</summary>
    public SegmentoArrecadacao? Segmento { get; init; }

    /// <summary>Valor do boleto em reais. <c>0</c> quando não informado.</summary>
    public decimal Valor { get; init; }

    /// <summary><c>true</c> quando o código traz um valor maior que zero.</summary>
    public bool ValorInformadoNoCodigo { get; init; }

    /// <summary>
    /// Multa. O código de barras do padrão não carrega multa, então é sempre
    /// <c>0</c>; a propriedade existe para o contrato de retorno.
    /// </summary>
    public decimal ValorMulta { get; init; }

    /// <summary>
    /// Desconto. O código de barras do padrão não carrega desconto, então é
    /// sempre <c>0</c>; a propriedade existe para o contrato de retorno.
    /// </summary>
    public decimal ValorDesconto { get; init; }

    /// <summary>
    /// Vencimento efetivo: a data do código (ou D+1 estimada) levada para a
    /// segunda-feira quando cair em sábado ou domingo. Feriados não são tratados.
    /// </summary>
    public DateOnly? DataVencimento { get; init; }

    /// <summary>Data como veio do código, antes do ajuste de sábado/domingo.</summary>
    public DateOnly? DataVencimentoOriginal { get; init; }

    /// <summary>
    /// <see cref="DataVencimento"/> como data e hora: meia-noite (00:00:00) do dia, com
    /// <see cref="DateTimeKind.Unspecified"/>, na data local do negócio e sem conversão para UTC.
    /// <c>null</c> quando não há vencimento. Calculado a partir de <see cref="DataVencimento"/>, então
    /// as duas representações nunca divergem.
    /// </summary>
    public DateTime? DataHoraVencimento => DataVencimento?.ToDateTime(TimeOnly.MinValue);

    /// <summary>
    /// <see cref="DataVencimentoOriginal"/> como data e hora (00:00:00, <see cref="DateTimeKind.Unspecified"/>).
    /// <c>null</c> quando não há vencimento. Calculado a partir de <see cref="DataVencimentoOriginal"/>.
    /// </summary>
    public DateTime? DataHoraVencimentoOriginal => DataVencimentoOriginal?.ToDateTime(TimeOnly.MinValue);

    /// <summary><c>true</c> quando <see cref="DataVencimento"/> difere de <see cref="DataVencimentoOriginal"/>.</summary>
    public bool VencimentoAjustadoDiaUtil { get; init; }

    /// <summary><c>true</c> quando a data é a estimativa D+1, não o vencimento do título.</summary>
    public bool VencimentoEstimado { get; init; }

    /// <summary>De onde veio <see cref="DataVencimento"/>.</summary>
    public OrigemVencimento OrigemVencimento { get; init; }

    /// <summary>Ciclo do fator usado; <see cref="CicloFatorVencimento.Nenhum"/> se o vencimento não veio do fator.</summary>
    public CicloFatorVencimento CicloFator { get; init; }

    /// <summary><c>true</c> quando <see cref="DataVencimento"/> é anterior a hoje (data local).</summary>
    public bool Vencido { get; init; }

    /// <summary>
    /// Hipótese de produto: boleto bancário com fator e valor zerados, como os de
    /// cartão de crédito. Não interfere em <see cref="IsValid"/>.
    /// </summary>
    public bool PossivelBoletoCartao { get; init; }

    /// <summary>Erros e avisos; só os bloqueantes derrubam <see cref="IsValid"/>.</summary>
    public IReadOnlyList<BoletoValidacaoError> Errors { get; init; } = [];
}
