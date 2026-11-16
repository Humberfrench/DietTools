namespace Dietcode.Core.Lib.Boletos;

/// <summary>Políticas da validação. Os padrões seguem a SPEC e as decisões de 16/11/2026.</summary>
public sealed class BoletoValidacaoOptions
{
    /// <summary>
    /// Quando <c>true</c> (padrão), <c>BOLETO_VENCIDO</c> é bloqueante e deixa
    /// <see cref="BoletoValidacaoResult.IsValid"/> em <c>false</c>. Com <c>false</c>
    /// o erro continua no resultado, mas sem bloquear.
    /// </summary>
    public bool BloquearVencido { get; set; } = true;

    /// <summary>
    /// Quando <c>true</c> (padrão), vencimento inexistente ou indeterminável vira
    /// D+1 da data de processamento, marcado como estimativa. Com <c>false</c>
    /// o resultado traz <c>VENCIMENTO_INDETERMINADO</c> bloqueante.
    /// </summary>
    public bool AtribuirDMaisUmQuandoVencimentoIndeterminado { get; set; } = true;

    /// <summary>
    /// Lê o vencimento do campo livre da arrecadação (AAAAMMDD nas posições
    /// 20–27) quando for uma data válida. É convenção dos convênios.
    /// </summary>
    public bool InterpretarDataDoCampoLivreDeArrecadacao { get; set; } = true;

    /// <summary>
    /// Para o fator ≥ 1000 usa-se o ciclo novo (22/02/2025 = 1000) enquanto a data
    /// ficar até esta quantidade de dias à frente de hoje; além disso, o fator é
    /// lido no ciclo original (passado). Padrão 5500, a janela da FEBRABAN
    /// (a confirmar no leiaute oficial).
    /// </summary>
    public int JanelaFuturaFatorDias { get; set; } = 5500;
}
