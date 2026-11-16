namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Segmento da arrecadação (posição 2 do código de barras). Mapa herdado de
/// <c>Dietcode.Core.Lib.Validator.IdentificarTipoDeBoleto</c>.
/// </summary>
public enum SegmentoArrecadacao
{
    /// <summary>Valor fora do mapa conhecido (<c>0</c> e <c>8</c>); não bloqueia a validação.</summary>
    Desconhecido = 0,

    /// <summary>Prefeituras.</summary>
    Prefeitura = 1,
    /// <summary>Convênios de saneamento.</summary>
    Saneamento = 2,
    /// <summary>Energia elétrica e gás.</summary>
    EnergiaEletricaGas = 3,
    /// <summary>Telecomunicações.</summary>
    Telecomunicacoes = 4,
    /// <summary>Órgãos governamentais.</summary>
    OrgaosGovernamentais = 5,

    /// <summary>Segmentos <c>6</c> e <c>9</c> (legado: "Outros").</summary>
    Outros = 6,

    /// <summary>Taxas e multas de trânsito.</summary>
    TaxasTransito = 7
}
