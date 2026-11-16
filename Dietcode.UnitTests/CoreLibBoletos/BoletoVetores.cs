using Dietcode.Core.Lib.Boletos;

namespace Dietcode.UnitTests.CoreLibBoletos;

/// <summary>
/// Códigos reais fornecidos pelo time em 16/11/2026 (todos bancários ou arrecadação
/// com indicador 6) mais vetores sintéticos para os casos que não há exemplo real.
/// </summary>
internal static class BoletoVetores
{
    /// <summary>Linhas digitáveis bancárias reais (47 dígitos). Fatores 1626–1630 = 10 a 14/11/2026.</summary>
    public static readonly string[] LinhasBancarias =
    [
        "17490000040000001504014551449144716300000004635",
        "42296011681000100427353046871512116260000321081",
        "23793190039002405878790002839703116260000636720",
        "23794130126009598057369001183404116260000031338",
        "34191090574103159730974128200008416260000020032",
        "03399504617690041984744270601022416290000711270",
        "03399504617690041676935472901020616260000294009",
        "03399504617690040879015471001022116280000146232",
        "42296010369000103213657024140915416260000570950"
    ];

    /// <summary>Códigos de barras de arrecadação reais (44 dígitos), segmento 1, indicador 6 (valor, Módulo 10).</summary>
    public static readonly string[] BarrasArrecadacao =
    [
        "81600000001428730112026111699202689338229010",
        "81620000001364630112026111699202689338234000",
        "81600000003840930112026111699202689338270010",
        "81600000001181430112026111699202689338417010",
        "81690000022329630112026123199202633273833001",
        "81610000006207130112026111699202689338473001",
        "81660000000993030112026111699202674021843010"
    ];

    // Sintéticos: códigos montados pelo algoritmo para os casos sem exemplo real
    // (indicadores 7/8/9, banco inexistente, moeda diferente, valores extremos). Provam
    // as regras do módulo e do fluxo, não um boleto emitido de verdade.

    /// <summary>Monta um código de barras bancário de 44 dígitos com o DV geral correto.</summary>
    public static string BarrasBancario(
        string banco = "341", char moeda = '9', int fator = 1626, long valorCentavos = 12345,
        string campoLivre = "1234567890123456789012345")
    {
        var semDv = banco + moeda + fator.ToString("0000") + valorCentavos.ToString("0000000000") + campoLivre;
        return semDv[..4] + DigitoVerificadorHelper.Modulo11Bancario(semDv[..4] + semDv[4..]) + semDv[4..];
    }

    /// <summary>Monta um código de barras de arrecadação de 44 dígitos com o DV geral correto.</summary>
    public static string BarrasArrecadacaoSintetica(
        char segmento = '1', char indicador = '6', long valorCentavos = 14287,
        string dados = "30112026111699202689338229010")
    {
        var semDv = "8" + segmento + indicador + valorCentavos.ToString("00000000000") + dados;
        var dv = DigitoVerificadorHelper.DigitoArrecadacao(indicador, semDv) ?? 0;
        return semDv[..3] + dv + semDv[3..];
    }

    /// <summary>Troca o caractere de <paramref name="indice"/> por outro dígito diferente.</summary>
    public static string Adulterar(string codigo, int indice)
    {
        var digito = codigo[indice] == '5' ? '6' : '5';
        return codigo[..indice] + digito + codigo[(indice + 1)..];
    }
}

/// <summary>Relógio fixo, em "hora local" sem fuso (UTC = local), para testar vencimento.</summary>
internal sealed class RelogioFixo : TimeProvider
{
    private readonly DateTimeOffset _agora;

    public RelogioFixo(int ano, int mes, int dia, int hora = 12)
        => _agora = new DateTimeOffset(ano, mes, dia, hora, 0, 0, TimeSpan.Zero);

    public override DateTimeOffset GetUtcNow() => _agora;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
