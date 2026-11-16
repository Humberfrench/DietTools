namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Regras da arrecadação (SPEC §6). Posições 1-based do código de barras: 1 = produto
/// (<c>8</c>), 2 = segmento, 3 = indicador de valor/referência e do módulo (<c>6</c>/<c>7</c>
/// Módulo 10, <c>8</c>/<c>9</c> Módulo 11), 4 = DV geral, 5–15 = valor, 16–44 = dados
/// do convênio. O fator bancário não existe aqui.
/// </summary>
internal sealed class BoletoArrecadacaoValidator : IFormatoBoletoValidator
{
    /// <inheritdoc />
    public FormatoBoletoResultado ValidarCodigoBarras(string codigoBarras, BoletoValidacaoOptions options)
    {
        var resultado = new FormatoBoletoResultado();
        AnalisarCodigoBarras(resultado, codigoBarras, options, dvsDivergentes: null);
        return resultado;
    }

    /// <inheritdoc />
    public FormatoBoletoResultado ValidarLinhaDigitavel(string linhaDigitavel, BoletoValidacaoOptions options)
    {
        var resultado = new FormatoBoletoResultado { LinhaDigitavel = linhaDigitavel };
        var codigoBarras = ConversorBoletoArrecadacao.ParaCodigoBarras(linhaDigitavel);
        var indicador = codigoBarras[2];
        var divergentes = new List<BoletoValidacaoError>();

        // Sem indicador válido não há como escolher o módulo; AnalisarCodigoBarras reporta.
        if (IndicadorValido(indicador))
        {
            string[] codigos =
            [
                BoletoValidacaoErrorCodes.ArrecadacaoCampo1Invalido,
                BoletoValidacaoErrorCodes.ArrecadacaoCampo2Invalido,
                BoletoValidacaoErrorCodes.ArrecadacaoCampo3Invalido,
                BoletoValidacaoErrorCodes.ArrecadacaoCampo4Invalido
            ];

            for (var bloco = 0; bloco < 4; bloco++)
            {
                var digitos = linhaDigitavel.Substring(bloco * 12, 11);
                var esperado = DigitoVerificadorHelper.DigitoArrecadacao(indicador, digitos);

                if (linhaDigitavel[(bloco * 12) + 11] - '0' != esperado)
                    divergentes.Add(new BoletoValidacaoError(
                        codigos[bloco], $"O dígito verificador do campo {bloco + 1} da linha digitável é inválido."));
            }
        }

        AnalisarCodigoBarras(resultado, codigoBarras, options, divergentes);
        return resultado;
    }

    /// <summary>
    /// <paramref name="dvsDivergentes"/> recebe os DVs de bloco já conferidos pela linha digitável; o DV geral é
    /// somado a eles e todos são resolvidos juntos em <see cref="ResolverDvs"/>.
    /// </summary>
    private static void AnalisarCodigoBarras(
        FormatoBoletoResultado resultado, string codigoBarras, BoletoValidacaoOptions options, List<BoletoValidacaoError>? dvsDivergentes)
    {
        dvsDivergentes ??= [];
        resultado.CodigoBarras = codigoBarras;

        var segmento = MapearSegmento(codigoBarras[1]);
        resultado.Segmento = segmento;
        if (segmento == SegmentoArrecadacao.Desconhecido)
            resultado.Errors.Add(new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.SegmentoDesconhecido,
                $"Segmento de arrecadação desconhecido: {codigoBarras[1]}.", Bloqueante: false));

        var indicador = codigoBarras[2];
        if (!IndicadorValido(indicador))
        {
            resultado.Errors.Add(new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.ArrecadacaoIndicadorInvalido,
                "O identificador de valor/referência (3º dígito) deve ser 6, 7, 8 ou 9."));
            return;
        }

        // DV geral (posição 4, índice 3) sobre os outros 43 dígitos.
        var semDv = codigoBarras[..3] + codigoBarras[4..];
        if (codigoBarras[3] - '0' != DigitoVerificadorHelper.DigitoArrecadacao(indicador, semDv))
            dvsDivergentes.Add(new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.ArrecadacaoDvInvalido, "O dígito verificador geral do código de barras é inválido."));

        ResolverDvs(resultado, indicador, dvsDivergentes, options);

        // Valor só existe nos indicadores 6 e 8; em 7 e 9 as posições 5–15 são referência.
        resultado.ValorCentavos = indicador is '6' or '8' ? long.Parse(codigoBarras.AsSpan(4, 11)) : 0;

        resultado.DataCampoLivre = FatorVencimentoHelper.LerDataAnoMesDia(codigoBarras.Substring(19, 8));
        resultado.LinhaDigitavel ??= ConversorBoletoArrecadacao.ParaLinhaDigitavel(codigoBarras);
    }

    /// <summary>
    /// DV divergente é erro bloqueante, exceto no Módulo 11 (indicador 8/9) com
    /// <see cref="BoletoValidacaoOptions.ValidarModulo11Arrecadacao"/> desligado: aí vira um único aviso.
    /// </summary>
    private static void ResolverDvs(
        FormatoBoletoResultado resultado, char indicador, List<BoletoValidacaoError> divergentes, BoletoValidacaoOptions options)
    {
        if (divergentes.Count == 0)
            return;

        if (indicador is '8' or '9' && !options.ValidarModulo11Arrecadacao)
        {
            resultado.Errors.Add(new BoletoValidacaoError(
                BoletoValidacaoErrorCodes.ArrecadacaoDvNaoConferido,
                "O dígito verificador (Módulo 11) não confere, mas a validação do Módulo 11 da arrecadação está desligada; valor e vencimento foram extraídos mesmo assim.",
                Bloqueante: false));
            return;
        }

        resultado.Errors.AddRange(divergentes);
    }

    private static bool IndicadorValido(char indicador) => indicador is >= '6' and <= '9';

    /// <summary>Mapa de segmentos herdado de <c>Validator.IdentificarTipoDeBoleto</c>.</summary>
    private static SegmentoArrecadacao MapearSegmento(char segmento)
        => segmento switch
        {
            '1' => SegmentoArrecadacao.Prefeitura,
            '2' => SegmentoArrecadacao.Saneamento,
            '3' => SegmentoArrecadacao.EnergiaEletricaGas,
            '4' => SegmentoArrecadacao.Telecomunicacoes,
            '5' => SegmentoArrecadacao.OrgaosGovernamentais,
            '6' or '9' => SegmentoArrecadacao.Outros,
            '7' => SegmentoArrecadacao.TaxasTransito,
            _ => SegmentoArrecadacao.Desconhecido
        };
}
