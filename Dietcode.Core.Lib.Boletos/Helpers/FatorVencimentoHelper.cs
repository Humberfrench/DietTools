namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Fator de vencimento e datas (SPEC §5). Substitui <c>Validator.ObterData</c> e
/// <c>ObterData2</c>, que só conheciam a base de 1997 e devolviam 07/10/1997 para
/// fator inválido.
/// </summary>
internal static class FatorVencimentoHelper
{
    /// <summary>Fator 0000 do ciclo original (não usado como data: o fator 0000 significa "sem vencimento").</summary>
    public static readonly DateOnly BaseCicloOriginal = new(1997, 10, 7);

    /// <summary>Data em que o fator voltou a 1000.</summary>
    public static readonly DateOnly InicioCicloNovo = new(2025, 2, 22);

    public const int FatorInicialCicloNovo = 1000;

    /// <summary>Janela padrão à frente de hoje (a da FEBRABAN, a confirmar no leiaute oficial).</summary>
    public const int JanelaFuturaPadraoDias = 5500;

    /// <summary>Limite superior da janela (100 anos); valores acima são limitados a ele.</summary>
    public const int JanelaFuturaMaximaDias = 36500;

    /// <summary>Resultado de <see cref="CalcularVencimento"/>.</summary>
    /// <param name="Data">Data obtida, ou <c>null</c> se o fator não a define.</param>
    /// <param name="Ciclo">Ciclo escolhido; <see cref="CicloFatorVencimento.Nenhum"/> sem data.</param>
    /// <param name="FatorInvalido"><c>true</c> para fatores 1 a 999, que não existem em nenhum ciclo em uso.</param>
    public readonly record struct ResultadoFator(DateOnly? Data, CicloFatorVencimento Ciclo, bool FatorInvalido);

    /// <summary>
    /// Converte o fator em data. Política (só boletos futuros são avaliados):
    /// <list type="bullet">
    /// <item><c>0000</c>: sem data (o chamador decide, p.ex. D+1).</item>
    /// <item><c>0001</c>–<c>0999</c>: inválido.</item>
    /// <item>≥ <c>1000</c>: ciclo novo (<c>22/02/2025 + fator − 1000</c>) desde que a data
    /// não passe de <paramref name="hoje"/> + <paramref name="janelaFuturaDias"/>; além disso,
    /// ciclo original (<c>07/10/1997 + fator</c>), que é sempre passado.</item>
    /// </list>
    /// </summary>
    /// <param name="fator">Fator de 4 dígitos (0–9999).</param>
    /// <param name="hoje">Data local de referência.</param>
    /// <param name="janelaFuturaDias">Limite à frente de hoje para aceitar o ciclo novo; negativo vira <see cref="JanelaFuturaPadraoDias"/> e acima de <see cref="JanelaFuturaMaximaDias"/> é limitado.</param>
    public static ResultadoFator CalcularVencimento(int fator, DateOnly hoje, int janelaFuturaDias)
    {
        if (fator == 0)
            return new ResultadoFator(null, CicloFatorVencimento.Nenhum, false);

        if (fator < FatorInicialCicloNovo)
            return new ResultadoFator(null, CicloFatorVencimento.Nenhum, true);

        // Janela negativa é configuração inválida: volta ao padrão em vez de empurrar tudo para o ciclo original.
        var janela = janelaFuturaDias < 0 ? JanelaFuturaPadraoDias : Math.Min(janelaFuturaDias, JanelaFuturaMaximaDias);
        var novo = InicioCicloNovo.AddDays(fator - FatorInicialCicloNovo);
        if (novo <= hoje.AddDays(janela))
            return new ResultadoFator(novo, CicloFatorVencimento.Novo, false);

        return new ResultadoFator(BaseCicloOriginal.AddDays(fator), CicloFatorVencimento.Original, false);
    }

    /// <summary>Leva sábado e domingo para a segunda-feira seguinte. Feriados não são tratados.</summary>
    public static DateOnly AjustarParaDiaUtil(DateOnly data)
        => data.DayOfWeek switch
        {
            DayOfWeek.Saturday => data.AddDays(2),
            DayOfWeek.Sunday => data.AddDays(1),
            _ => data
        };

    /// <summary>
    /// Lê uma data AAAAMMDD de 8 dígitos (campo livre da arrecadação). Aceita anos
    /// 2000–2099 para não confundir dígitos quaisquer com data.
    /// </summary>
    public static DateOnly? LerDataAnoMesDia(string oitoDigitos)
    {
        if (oitoDigitos.Length != 8
            || !int.TryParse(oitoDigitos.AsSpan(0, 4), out var ano)
            || !int.TryParse(oitoDigitos.AsSpan(4, 2), out var mes)
            || !int.TryParse(oitoDigitos.AsSpan(6, 2), out var dia)
            || ano is < 2000 or > 2099
            || mes is < 1 or > 12
            || dia < 1 || dia > DateTime.DaysInMonth(ano, mes))
        {
            return null;
        }

        return new DateOnly(ano, mes, dia);
    }
}
