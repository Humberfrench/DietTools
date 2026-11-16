namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Ciclo do fator de vencimento usado para converter o fator em data. O fator
/// reiniciou em <c>1000</c> em 22/02/2025, então todo fator ≥ 1000 tem duas
/// datas possíveis; a escolha é política do validador (SPEC §5).
/// </summary>
public enum CicloFatorVencimento
{
    /// <summary>O vencimento não veio do fator.</summary>
    Nenhum = 0,

    /// <summary>Ciclo original: base 07/10/1997, fator 9999 = 21/02/2025.</summary>
    Original = 1,

    /// <summary>Novo ciclo: fator 1000 = 22/02/2025.</summary>
    Novo = 2
}
