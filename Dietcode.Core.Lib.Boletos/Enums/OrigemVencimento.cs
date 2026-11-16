namespace Dietcode.Core.Lib.Boletos;

/// <summary>De onde veio <see cref="BoletoValidacaoResult.DataVencimento"/>.</summary>
public enum OrigemVencimento
{
    /// <summary>Nenhuma data foi obtida (vencimento indeterminado e sem regra D+1).</summary>
    NaoInformado = 0,

    /// <summary>Calculada a partir do fator de vencimento (boleto bancário).</summary>
    FatorVencimento = 1,

    /// <summary>
    /// Lida do campo livre da arrecadação (AAAAMMDD nas posições 20–27 do código
    /// de barras). É convenção dos convênios, não garantia do padrão.
    /// </summary>
    CampoLivreArrecadacao = 2,

    /// <summary>Estimativa do gateway: dia seguinte à data de processamento. Não é o vencimento original.</summary>
    RegraDMaisUm = 3
}
