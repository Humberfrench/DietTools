using Microsoft.Extensions.Options;

namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Orquestra normalização, classificação, validação do formato e as políticas de
/// vencimento e valor (SPEC §3 a §6). Sem estado além das opções e do relógio; seguro
/// para uso como singleton.
/// </summary>
public sealed class BoletoValidator : IBoletoValidator
{
    private readonly BoletoValidatorFactory _factory = new();
    private readonly TimeProvider _timeProvider;
    private readonly BoletoValidacaoOptions _optionsPadrao;

    /// <summary>Usa o relógio do sistema e as políticas padrão.</summary>
    public BoletoValidator()
        : this(TimeProvider.System, Options.Create(new BoletoValidacaoOptions()))
    {
    }

    /// <summary>
    /// A data "de hoje" é a data local de <paramref name="timeProvider"/> (sem UTC), nunca
    /// <c>DateTime.Now</c>; compara-se só a data, não o horário.
    /// </summary>
    public BoletoValidator(TimeProvider timeProvider, IOptions<BoletoValidacaoOptions> options)
    {
        _timeProvider = timeProvider;
        _optionsPadrao = options.Value;
    }

    /// <inheritdoc />
    public TipoCodigoBoleto IdentificarTipo(string? codigo)
        => CodigoBoletoNormalizer.Normalizar(codigo, out var normalizado) is null
            ? CodigoBoletoNormalizer.Classificar(normalizado)
            : TipoCodigoBoleto.NaoSuportado;

    /// <inheritdoc />
    public BoletoValidacaoResult Validar(string? codigo, BoletoValidacaoOptions? options = null)
    {
        options ??= _optionsPadrao;
        var original = codigo ?? string.Empty;

        var erroNormalizacao = CodigoBoletoNormalizer.Normalizar(codigo, out var normalizado);
        if (erroNormalizacao is not null)
            return Falha(original, normalizado, TipoCodigoBoleto.NaoSuportado, erroNormalizacao);

        var tipo = CodigoBoletoNormalizer.Classificar(normalizado);
        if (tipo == TipoCodigoBoleto.NaoSuportado)
            return Falha(original, normalizado, tipo, ErroDeTipoNaoSuportado(normalizado));

        var validador = _factory.ObterValidador(tipo);
        var formato = tipo is TipoCodigoBoleto.CodigoBarrasBancario or TipoCodigoBoleto.CodigoBarrasArrecadacao
            ? validador.ValidarCodigoBarras(normalizado, options)
            : validador.ValidarLinhaDigitavel(normalizado, options);

        var erros = new List<BoletoValidacaoError>(formato.Errors);
        var vencimento = ResolverVencimento(formato, options, erros);
        var valor = formato.ValorCentavos / 100m;

        if (valor == 0)
            erros.Add(new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.ValorNaoInformado, "O código não informa o valor do boleto.", Bloqueante: false));

        return new BoletoValidacaoResult
        {
            IsValid = !erros.Any(e => e.Bloqueante),
            TipoCodigo = tipo,
            CodigoOriginal = original,
            CodigoNormalizado = normalizado,
            CodigoBarras = formato.CodigoBarras,
            LinhaDigitavel = formato.LinhaDigitavel,
            BancoCodigo = formato.BancoCodigo,
            Segmento = formato.Segmento,
            Valor = valor,
            ValorInformadoNoCodigo = valor > 0,
            DataVencimento = vencimento.Data,
            DataVencimentoOriginal = vencimento.DataOriginal,
            DataHoraVencimento = vencimento.Data?.ToDateTime(TimeOnly.MinValue),
            DataHoraVencimentoOriginal = vencimento.DataOriginal?.ToDateTime(TimeOnly.MinValue),
            VencimentoAjustadoDiaUtil = vencimento.DataOriginal != vencimento.Data,
            VencimentoEstimado = vencimento.Estimado,
            OrigemVencimento = vencimento.Origem,
            CicloFator = vencimento.Ciclo,
            Vencido = vencimento.Vencido,
            PossivelBoletoCartao = formato.PossivelBoletoCartao,
            Errors = erros
        };
    }

    private readonly record struct Vencimento(
        DateOnly? Data,
        DateOnly? DataOriginal,
        OrigemVencimento Origem,
        CicloFatorVencimento Ciclo,
        bool Estimado,
        bool Vencido);

    private Vencimento ResolverVencimento(
        FormatoBoletoResultado formato, BoletoValidacaoOptions options, List<BoletoValidacaoError> erros)
    {
        var hoje = ObterHoje(options);

        DateOnly? data = null;
        var origem = OrigemVencimento.NaoInformado;
        var ciclo = CicloFatorVencimento.Nenhum;

        if (formato.Fator is int fator)
        {
            var calculo = FatorVencimentoHelper.CalcularVencimento(fator, hoje, options.JanelaFuturaFatorDias);

            if (calculo.FatorInvalido)
            {
                erros.Add(new BoletoValidacaoError(
                    BoletoValidacaoErrorCodes.FatorVencimentoInvalido,
                    $"Fator de vencimento inválido: {fator:0000}. Fatores de 0001 a 0999 não existem nos ciclos em uso."));

                return new Vencimento(null, null, OrigemVencimento.NaoInformado, CicloFatorVencimento.Nenhum, false, false);
            }

            data = calculo.Data;
            ciclo = calculo.Ciclo;
            if (data is not null)
                origem = OrigemVencimento.FatorVencimento;
        }
        else if (options.InterpretarDataDoCampoLivreDeArrecadacao && formato.DataCampoLivre is not null)
        {
            data = formato.DataCampoLivre;
            origem = OrigemVencimento.CampoLivreArrecadacao;
        }

        var estimado = false;
        if (data is null)
        {
            if (!options.AtribuirDMaisUmQuandoVencimentoIndeterminado)
            {
                erros.Add(new BoletoValidacaoError(
                    BoletoValidacaoErrorCodes.VencimentoIndeterminado, "O código não define a data de vencimento."));

                return new Vencimento(null, null, OrigemVencimento.NaoInformado, CicloFatorVencimento.Nenhum, false, false);
            }

            data = hoje.AddDays(1);
            origem = OrigemVencimento.RegraDMaisUm;
            estimado = true;
            erros.Add(new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.VencimentoIndeterminado,
                "O código não define a data de vencimento; foi atribuído D+1 da data de processamento (estimativa).",
                Bloqueante: false));
        }

        var original = data.Value;
        var efetivo = FatorVencimentoHelper.AjustarParaDiaUtil(original);
        var vencido = efetivo < hoje;

        if (vencido)
            erros.Add(new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.BoletoVencido,
                $"O boleto venceu em {efetivo:dd/MM/yyyy}.", options.BloquearVencido));

        return new Vencimento(efetivo, original, origem, ciclo, estimado, vencido);
    }

    /// <summary>Data de hoje: local do <see cref="TimeProvider"/> ou, se configurado, a do fuso do negócio.</summary>
    private DateOnly ObterHoje(BoletoValidacaoOptions options)
    {
        var agora = string.IsNullOrWhiteSpace(options.FusoHorarioId)
            ? _timeProvider.GetLocalNow()
            : TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), TimeZoneInfo.FindSystemTimeZoneById(options.FusoHorarioId));

        return DateOnly.FromDateTime(agora.DateTime);
    }

    private static BoletoValidacaoError ErroDeTipoNaoSuportado(string normalizado)
        => normalizado.Length == 48
            ? new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.TipoNaoSuportado, "Linha digitável de 48 dígitos só é suportada para arrecadação (iniciada em 8).")
            : new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.TamanhoInvalido,
                $"Tamanho inválido: {normalizado.Length} dígitos. Use 44 (código de barras), 47 (linha bancária) ou 48 (linha de arrecadação).");

    private static BoletoValidacaoResult Falha(
        string original, string normalizado, TipoCodigoBoleto tipo, BoletoValidacaoError erro)
        => new()
        {
            IsValid = false,
            TipoCodigo = tipo,
            CodigoOriginal = original,
            CodigoNormalizado = normalizado,
            Errors = [erro]
        };
}
