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
    /// Quando <c>true</c>, o dígito verificador divergente da arrecadação com indicador <c>8</c> ou
    /// <c>9</c> (Módulo 11) é erro bloqueante. Padrão <c>false</c>: vira o aviso não bloqueante
    /// <c>ARRECADACAO_DV_NAO_CONFERIDO</c> e valor e vencimento seguem sendo extraídos. O Módulo 11 da
    /// arrecadação ainda não foi conferido com um boleto real; os indicadores 6 e 7 (Módulo 10) sempre
    /// validam.
    /// </summary>
    public bool ValidarModulo11Arrecadacao { get; set; }

    /// <summary>
    /// Fuso horário do negócio (id do sistema, ex.: <c>America/Sao_Paulo</c>) para decidir a data de "hoje".
    /// Padrão <c>null</c>: usa a data local do <see cref="TimeProvider"/> (sem UTC). Em host configurado em UTC
    /// (contêineres, nuvem), defina o fuso, ou a data vira o dia seguinte no fim da tarde de Brasília.
    /// Um id inexistente lança <see cref="TimeZoneNotFoundException"/> na primeira validação.
    /// </summary>
    public string? FusoHorarioId { get; set; }

    /// <summary>
    /// Para o fator ≥ 1000 usa-se o ciclo novo (22/02/2025 = 1000) enquanto a data
    /// ficar até esta quantidade de dias à frente de hoje; além disso, o fator é
    /// lido no ciclo original (passado). Padrão 5500, a janela da FEBRABAN
    /// (a confirmar no leiaute oficial). Negativo volta ao padrão; acima de 36500 é limitado a 36500.
    /// </summary>
    public int JanelaFuturaFatorDias { get; set; } = FatorVencimentoHelper.JanelaFuturaPadraoDias;
}
