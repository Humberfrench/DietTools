using Dietcode.Core.Lib.Boletos;
using Dietcode.Core.Lib.Boletos.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dietcode.UnitTests.CoreLibBoletos;

public class BoletoValidatorTests
{
    // Hoje = 09/11/2026 (segunda): todos os boletos reais (fatores 1626-1630, 10 a 14/11) ainda vencem.
    private static BoletoValidator Validador(int ano = 2026, int mes = 11, int dia = 9)
        => new(new RelogioFixo(ano, mes, dia), Microsoft.Extensions.Options.Options.Create(new BoletoValidacaoOptions()));

    private static bool TemErro(BoletoValidacaoResult r, string codigo, bool? bloqueante = null)
        => r.Errors.Any(e => e.Code == codigo && (bloqueante is null || e.Bloqueante == bloqueante));

    // ---- identificação e entrada --------------------------------------------------------

    [Fact]
    public void IdentificarTipo_ReconheceOsQuatroFormatos()
    {
        var v = Validador();

        Assert.Equal(TipoCodigoBoleto.LinhaDigitavelBancaria, v.IdentificarTipo(BoletoVetores.LinhasBancarias[0]));
        Assert.Equal(TipoCodigoBoleto.CodigoBarrasBancario, v.IdentificarTipo(BoletoVetores.BarrasBancario()));
        Assert.Equal(TipoCodigoBoleto.CodigoBarrasArrecadacao, v.IdentificarTipo(BoletoVetores.BarrasArrecadacao[0]));
        Assert.Equal(TipoCodigoBoleto.LinhaDigitavelArrecadacao,
            v.IdentificarTipo(ConversorBoletoArrecadacao.ParaLinhaDigitavel(BoletoVetores.BarrasArrecadacao[0])));
        Assert.Equal(TipoCodigoBoleto.NaoSuportado, v.IdentificarTipo("123"));
        Assert.Equal(TipoCodigoBoleto.NaoSuportado, v.IdentificarTipo(null));
    }

    [Theory]
    [InlineData(43)]
    [InlineData(45)]
    [InlineData(46)]
    [InlineData(49)]
    public void TamanhosForaDoPadrao_SaoRejeitados_Inclusive46(int tamanho)
    {
        var r = Validador().Validar(new string('1', tamanho));

        Assert.False(r.IsValid);
        Assert.Equal(TipoCodigoBoleto.NaoSuportado, r.TipoCodigo);
        Assert.Equal(BoletoValidacaoErrorCodes.TamanhoInvalido, Assert.Single(r.Errors).Code);
    }

    [Fact]
    public void Linha48_QueNaoComecaCom8_NaoEhSuportada()
    {
        var r = Validador().Validar("1" + new string('0', 47));

        Assert.False(r.IsValid);
        Assert.Equal(BoletoValidacaoErrorCodes.TipoNaoSuportado, Assert.Single(r.Errors).Code);
    }

    [Theory]
    [InlineData(null, BoletoValidacaoErrorCodes.CodigoVazio)]
    [InlineData("   ", BoletoValidacaoErrorCodes.CodigoVazio)]
    [InlineData("0339950461769004198474427060102241629000071127A", BoletoValidacaoErrorCodes.CodigoInvalido)]
    public void EntradaVaziaOuComLetras_Rejeita(string? entrada, string codigoErro)
    {
        var r = Validador().Validar(entrada);

        Assert.False(r.IsValid);
        Assert.Equal(codigoErro, Assert.Single(r.Errors).Code);
        Assert.Equal(entrada ?? string.Empty, r.CodigoOriginal);
    }

    [Fact]
    public void Separadores_SaoNormalizados_SemPerderZerosAEsquerda()
    {
        var r = Validador().Validar("03399.50461 76900.419847 44270.601022 4 16290000711270");

        Assert.True(r.IsValid);
        Assert.Equal("03399504617690041984744270601022416290000711270", r.CodigoNormalizado);
        Assert.StartsWith("0339", r.CodigoBarras);
    }

    // ---- bancário: boletos reais --------------------------------------------------------

    [Fact]
    public void LinhaBancariaReal_ValidaEExtraiValorEVencimento()
    {
        // Fator 1630 = sábado 14/11/2026 -> vence na segunda 16/11.
        var r = Validador().Validar("17490000040000001504014551449144716300000004635");

        Assert.True(r.IsValid);
        Assert.Equal(TipoCodigoBoleto.LinhaDigitavelBancaria, r.TipoCodigo);
        Assert.Equal("174", r.BancoCodigo);
        Assert.Equal(46.35m, r.Valor);
        Assert.True(r.ValorInformadoNoCodigo);
        Assert.Equal(new DateOnly(2026, 11, 14), r.DataVencimentoOriginal);
        Assert.Equal(new DateOnly(2026, 11, 16), r.DataVencimento);
        Assert.True(r.VencimentoAjustadoDiaUtil);
        Assert.Equal(OrigemVencimento.FatorVencimento, r.OrigemVencimento);
        Assert.Equal(CicloFatorVencimento.Novo, r.CicloFator);
        Assert.False(r.VencimentoEstimado);
        Assert.False(r.Vencido);
        Assert.Equal(44, r.CodigoBarras!.Length);
    }

    [Fact]
    public void TodasAsLinhasBancariasReais_SaoValidas_EFazemRoundTripCom44()
    {
        var v = Validador();

        foreach (var linha in BoletoVetores.LinhasBancarias)
        {
            var daLinha = v.Validar(linha);
            Assert.True(daLinha.IsValid, linha);
            Assert.Equal(linha, daLinha.LinhaDigitavel);

            var dasBarras = v.Validar(daLinha.CodigoBarras);
            Assert.True(dasBarras.IsValid, linha);
            Assert.Equal(TipoCodigoBoleto.CodigoBarrasBancario, dasBarras.TipoCodigo);
            Assert.Equal(linha, dasBarras.LinhaDigitavel);
            Assert.Equal(daLinha.Valor, dasBarras.Valor);
            Assert.Equal(daLinha.DataVencimento, dasBarras.DataVencimento);
        }
    }

    [Fact]
    public void BoletoReal_Vencido_BloqueiaPorPadrao_EAvisaQuandoNaoBloqueia()
    {
        // 16/11/2026: o fator 1626 (10/11) já passou.
        var v = Validador(2026, 11, 16);
        var linha = BoletoVetores.LinhasBancarias[1];

        var padrao = v.Validar(linha);
        Assert.False(padrao.IsValid);
        Assert.True(padrao.Vencido);
        Assert.True(TemErro(padrao, BoletoValidacaoErrorCodes.BoletoVencido, bloqueante: true));

        var semBloqueio = v.Validar(linha, new BoletoValidacaoOptions { BloquearVencido = false });
        Assert.True(semBloqueio.IsValid);
        Assert.True(semBloqueio.Vencido);
        Assert.True(TemErro(semBloqueio, BoletoValidacaoErrorCodes.BoletoVencido, bloqueante: false));
    }

    [Fact]
    public void BoletoQueVenceHoje_Eh_Aceito()
    {
        // Fator 1626 = terça 10/11/2026.
        var r = Validador(2026, 11, 10).Validar(BoletoVetores.LinhasBancarias[1]);

        Assert.True(r.IsValid);
        Assert.False(r.Vencido);
    }

    [Fact]
    public void MultaEDesconto_VoltamZerados()
    {
        var r = Validador().Validar(BoletoVetores.LinhasBancarias[0]);

        Assert.Equal(0m, r.ValorMulta);
        Assert.Equal(0m, r.ValorDesconto);
    }

    // ---- bancário: dígitos, moeda e banco -----------------------------------------------

    [Fact]
    public void DvGeralAlterado_Rejeita()
    {
        var barras = BoletoVetores.BarrasBancario();
        var r = Validador().Validar(BoletoVetores.Adulterar(barras, 4));

        Assert.False(r.IsValid);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.DvInvalido));
    }

    [Theory]
    [InlineData(9, BoletoValidacaoErrorCodes.Campo1Invalido)]
    [InlineData(20, BoletoValidacaoErrorCodes.Campo2Invalido)]
    [InlineData(31, BoletoValidacaoErrorCodes.Campo3Invalido)]
    public void DvDeCadaCampoDaLinha_Alterado_RejeitaSoAqueleCampo(int indice, string esperado)
    {
        var r = Validador().Validar(BoletoVetores.Adulterar(BoletoVetores.LinhasBancarias[0], indice));
        var codigos = r.Errors.Where(e => e.Bloqueante).Select(e => e.Code).ToArray();

        Assert.False(r.IsValid);
        Assert.Equal([esperado], codigos);
    }

    [Fact]
    public void MoedaDiferenteDe9_Rejeita()
    {
        var r = Validador().Validar(BoletoVetores.BarrasBancario(moeda: '0'));

        Assert.False(r.IsValid);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.MoedaInvalida));
    }

    [Fact]
    public void BancoDesconhecido_Eh_Ignorado_ESegueParaValorEData()
    {
        var r = Validador().Validar(BoletoVetores.BarrasBancario(banco: "999", fator: 1630, valorCentavos: 5000));

        Assert.True(r.IsValid);
        Assert.Equal("999", r.BancoCodigo);
        Assert.Equal(50m, r.Valor);
        Assert.Equal(new DateOnly(2026, 11, 16), r.DataVencimento);
    }

    // ---- valor e fator ------------------------------------------------------------------

    [Fact]
    public void Valor0000012345_Eh_123e45()
    {
        var r = Validador().Validar(BoletoVetores.BarrasBancario(valorCentavos: 12345));

        Assert.Equal(123.45m, r.Valor);
    }

    [Fact]
    public void ValorDe10Digitos_NaoEstouraInt()
    {
        var r = Validador().Validar(BoletoVetores.BarrasBancario(valorCentavos: 9_999_999_999));

        Assert.Equal(99_999_999.99m, r.Valor);
    }

    [Fact]
    public void ValorZero_VemZero_ComAvisoNaoBloqueante()
    {
        var r = Validador().Validar(BoletoVetores.BarrasBancario(valorCentavos: 0));

        Assert.True(r.IsValid);
        Assert.Equal(0m, r.Valor);
        Assert.False(r.ValorInformadoNoCodigo);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.ValorNaoInformado, bloqueante: false));
    }

    [Fact]
    public void Fator0000_UsaDMaisUm_MarcadoComoEstimativa()
    {
        // Hoje = sexta 13/11/2026 -> D+1 = sábado -> segunda 16/11.
        var r = Validador(2026, 11, 13).Validar(BoletoVetores.BarrasBancario(fator: 0));

        Assert.True(r.IsValid);
        Assert.True(r.VencimentoEstimado);
        Assert.Equal(OrigemVencimento.RegraDMaisUm, r.OrigemVencimento);
        Assert.Equal(new DateOnly(2026, 11, 14), r.DataVencimentoOriginal);
        Assert.Equal(new DateOnly(2026, 11, 16), r.DataVencimento);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.VencimentoIndeterminado, bloqueante: false));
    }

    [Fact]
    public void Fator0000_SemRegraDMaisUm_Bloqueia()
    {
        var r = Validador().Validar(
            BoletoVetores.BarrasBancario(fator: 0),
            new BoletoValidacaoOptions { AtribuirDMaisUmQuandoVencimentoIndeterminado = false });

        Assert.False(r.IsValid);
        Assert.Null(r.DataVencimento);
        Assert.Equal(OrigemVencimento.NaoInformado, r.OrigemVencimento);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.VencimentoIndeterminado, bloqueante: true));
    }

    [Fact]
    public void FatorEntre0001e0999_Eh_FatorInvalido()
    {
        var r = Validador().Validar(BoletoVetores.BarrasBancario(fator: 500));

        Assert.False(r.IsValid);
        Assert.Null(r.DataVencimento);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.FatorVencimentoInvalido));
    }

    [Fact]
    public void Fator9999_NoCicloOriginal_Vence_21_02_2025_EEstaVencido()
    {
        var r = Validador(2026, 11, 16).Validar(BoletoVetores.BarrasBancario(fator: 9999));

        Assert.Equal(CicloFatorVencimento.Original, r.CicloFator);
        Assert.Equal(new DateOnly(2025, 2, 21), r.DataVencimentoOriginal);
        Assert.True(r.Vencido);
    }

    [Theory]
    [InlineData(1000, 2025, 2, 22)]
    [InlineData(1001, 2025, 2, 23)]
    public void FatoresDoNovoCiclo_NoBoleto(int fator, int ano, int mes, int dia)
    {
        var r = Validador(2026, 11, 16).Validar(BoletoVetores.BarrasBancario(fator: fator));

        Assert.Equal(CicloFatorVencimento.Novo, r.CicloFator);
        Assert.Equal(new DateOnly(ano, mes, dia), r.DataVencimentoOriginal);
    }

    [Fact]
    public void BoletoComFatorEValorZerados_Eh_SinalizadoComoPossivelCartao()
    {
        var cartao = Validador().Validar(BoletoVetores.BarrasBancario(fator: 0, valorCentavos: 0));
        var comum = Validador().Validar(BoletoVetores.LinhasBancarias[0]);

        Assert.True(cartao.PossivelBoletoCartao);
        Assert.False(comum.PossivelBoletoCartao);
    }

    // ---- arrecadação --------------------------------------------------------------------

    private static readonly BoletoValidacaoOptions Modulo11Estrito = new() { ValidarModulo11Arrecadacao = true };

    [Fact]
    public void BarrasDeArrecadacaoReais_SaoValidas_ComValorEVencimentoDoCampoLivre()
    {
        var v = Validador();

        var primeira = v.Validar(BoletoVetores.BarrasArrecadacao[0]);
        Assert.True(primeira.IsValid);
        Assert.Equal(TipoCodigoBoleto.CodigoBarrasArrecadacao, primeira.TipoCodigo);
        Assert.Equal(SegmentoArrecadacao.Prefeitura, primeira.Segmento);
        Assert.Equal(142.87m, primeira.Valor);
        Assert.True(primeira.ValorInformadoNoCodigo);
        Assert.Equal(new DateOnly(2026, 11, 16), primeira.DataVencimento);
        Assert.Equal(OrigemVencimento.CampoLivreArrecadacao, primeira.OrigemVencimento);
        Assert.False(primeira.VencimentoEstimado);
        Assert.Equal(CicloFatorVencimento.Nenhum, primeira.CicloFator);

        var quinta = v.Validar(BoletoVetores.BarrasArrecadacao[4]);
        Assert.True(quinta.IsValid);
        Assert.Equal(2232.96m, quinta.Valor);
        Assert.Equal(new DateOnly(2026, 12, 31), quinta.DataVencimento);

        foreach (var barras in BoletoVetores.BarrasArrecadacao)
            Assert.True(v.Validar(barras).IsValid, barras);
    }

    [Fact]
    public void Arrecadacao_Linha48_FazRoundTripCom44()
    {
        var v = Validador();

        foreach (var barras in BoletoVetores.BarrasArrecadacao)
        {
            var linha = v.Validar(barras).LinhaDigitavel;
            Assert.Equal(48, linha!.Length);

            var daLinha = v.Validar(linha);
            Assert.True(daLinha.IsValid, barras);
            Assert.Equal(TipoCodigoBoleto.LinhaDigitavelArrecadacao, daLinha.TipoCodigo);
            Assert.Equal(barras, daLinha.CodigoBarras);
            Assert.Equal(v.Validar(barras).Valor, daLinha.Valor);
        }
    }

    [Fact]
    public void Arrecadacao_Linha48_Formatada_ComSeparadores()
    {
        var linha = Validador().Validar(BoletoVetores.BarrasArrecadacao[0]).LinhaDigitavel!;
        var formatada = $"{linha[..12]}-{linha[12..24]} {linha[24..36]}.{linha[36..]}";

        Assert.True(Validador().Validar(formatada).IsValid);
    }

    [Theory]
    [InlineData('6', true)]
    [InlineData('7', false)]
    [InlineData('8', true)]
    [InlineData('9', false)]
    public void Indicadores6a9_TemValorSoNos6e8(char indicador, bool temValor)
    {
        // Sintéticos: DV geral calculado pelo algoritmo (Módulo 10 para 6/7, Módulo 11 para 8/9).
        var r = Validador().Validar(BoletoVetores.BarrasArrecadacaoSintetica(indicador: indicador, valorCentavos: 14287));

        Assert.True(r.IsValid, indicador.ToString());
        Assert.Equal(temValor, r.ValorInformadoNoCodigo);
        Assert.Equal(temValor ? 142.87m : 0m, r.Valor);
        Assert.Equal(!temValor, TemErro(r, BoletoValidacaoErrorCodes.ValorNaoInformado, bloqueante: false));
    }

    [Theory]
    [InlineData('6')]
    [InlineData('7')]
    [InlineData('8')]
    [InlineData('9')]
    public void Arrecadacao_DvGeralAlterado_Rejeita(char indicador)
    {
        var barras = BoletoVetores.BarrasArrecadacaoSintetica(indicador: indicador);
        var r = Validador().Validar(BoletoVetores.Adulterar(barras, 3), Modulo11Estrito);

        Assert.False(r.IsValid);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.ArrecadacaoDvInvalido, bloqueante: true));
    }

    [Theory]
    [InlineData('6')]
    [InlineData('8')]
    public void Arrecadacao_DvDeCadaBlocoAlterado_RejeitaSoAqueleBloco(char indicador)
    {
        var barras = BoletoVetores.BarrasArrecadacaoSintetica(indicador: indicador);
        var linha = ConversorBoletoArrecadacao.ParaLinhaDigitavel(barras)!;
        string[] esperados =
        [
            BoletoValidacaoErrorCodes.ArrecadacaoCampo1Invalido, BoletoValidacaoErrorCodes.ArrecadacaoCampo2Invalido,
            BoletoValidacaoErrorCodes.ArrecadacaoCampo3Invalido, BoletoValidacaoErrorCodes.ArrecadacaoCampo4Invalido
        ];

        for (var bloco = 0; bloco < 4; bloco++)
        {
            var r = Validador().Validar(BoletoVetores.Adulterar(linha, (bloco * 12) + 11), Modulo11Estrito);

            Assert.Equal([esperados[bloco]], r.Errors.Where(e => e.Bloqueante).Select(e => e.Code).ToArray());
        }
    }

    [Fact]
    public void Arrecadacao_ModuloEscolhidoPeloIndicador_NaoEhSempreModulo10()
    {
        // Com indicador 8 o DV tem que ser o do Módulo 11; um DV de Módulo 10 diferente deve falhar.
        var corpo = "88" + "8" + "00000014287" + "30112026111699202689338229010";
        var dv10 = DigitoVerificadorHelper.Modulo10(corpo);
        var dv11 = DigitoVerificadorHelper.Modulo11Arrecadacao(corpo);
        Assert.NotEqual(dv10, dv11);

        var comMod11 = corpo[..3] + dv11 + corpo[3..];
        var comMod10 = corpo[..3] + dv10 + corpo[3..];

        Assert.True(Validador().Validar(comMod11, Modulo11Estrito).IsValid);
        Assert.True(TemErro(Validador().Validar(comMod10, Modulo11Estrito), BoletoValidacaoErrorCodes.ArrecadacaoDvInvalido, bloqueante: true));
    }

    [Theory]
    [InlineData('8')]
    [InlineData('9')]
    public void Arrecadacao_Modulo11Divergente_NoPadrao_AvisaMasExtraiValorEVencimento(char indicador)
    {
        var barras = BoletoVetores.BarrasArrecadacaoSintetica(indicador: indicador, valorCentavos: 14287);
        var r = Validador(2026, 11, 9).Validar(BoletoVetores.Adulterar(barras, 3));

        Assert.True(r.IsValid);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.ArrecadacaoDvNaoConferido, bloqueante: false));
        Assert.False(TemErro(r, BoletoValidacaoErrorCodes.ArrecadacaoDvInvalido));
        Assert.Equal(indicador == '8' ? 142.87m : 0m, r.Valor);
        Assert.Equal(new DateOnly(2026, 11, 16), r.DataVencimento);
    }

    [Fact]
    public void Arrecadacao_Modulo11Divergente_NaLinha_GeraUmUnicoAviso()
    {
        var barras = BoletoVetores.BarrasArrecadacaoSintetica(indicador: '8');
        var linha = ConversorBoletoArrecadacao.ParaLinhaDigitavel(barras)!;
        // Altera o DV de dois blocos e o geral (que está no bloco 1: posição 4).
        var adulterada = BoletoVetores.Adulterar(BoletoVetores.Adulterar(BoletoVetores.Adulterar(linha, 11), 23), 3);

        var r = Validador(2026, 11, 9).Validar(adulterada);

        Assert.True(r.IsValid);
        Assert.Single(r.Errors, e => e.Code == BoletoValidacaoErrorCodes.ArrecadacaoDvNaoConferido);
        Assert.Equal(142.87m, r.Valor);
    }

    [Theory]
    [InlineData('6')]
    [InlineData('7')]
    public void Arrecadacao_Modulo10Divergente_BloqueiaMesmoNoPadrao(char indicador)
    {
        var barras = BoletoVetores.BarrasArrecadacaoSintetica(indicador: indicador);
        var r = Validador().Validar(BoletoVetores.Adulterar(barras, 3));

        Assert.False(r.IsValid);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.ArrecadacaoDvInvalido, bloqueante: true));
        Assert.False(TemErro(r, BoletoValidacaoErrorCodes.ArrecadacaoDvNaoConferido));
    }

    [Fact]
    public void Arrecadacao_Modulo11Correto_NaoGeraAviso_NemNoPadrao()
    {
        var r = Validador(2026, 11, 9).Validar(BoletoVetores.BarrasArrecadacaoSintetica(indicador: '8'));

        Assert.True(r.IsValid);
        Assert.False(TemErro(r, BoletoValidacaoErrorCodes.ArrecadacaoDvNaoConferido));
    }

    [Fact]
    public void Arrecadacao_ValorDe11Digitos_AcimaDeIntMax_NaoEstoura()
    {
        var r = Validador().Validar(BoletoVetores.BarrasArrecadacaoSintetica(valorCentavos: 99_999_999_999));

        Assert.True(r.IsValid);
        Assert.Equal(999_999_999.99m, r.Valor);
    }

    [Fact]
    public void Arrecadacao_IndicadorInvalido_Rejeita()
    {
        var r = Validador().Validar(BoletoVetores.BarrasArrecadacaoSintetica(indicador: '5'));

        Assert.False(r.IsValid);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.ArrecadacaoIndicadorInvalido));
    }

    [Theory]
    [InlineData('0')]
    [InlineData('8')]
    public void Arrecadacao_SegmentoDesconhecido_NaoBloqueia(char segmento)
    {
        var r = Validador().Validar(BoletoVetores.BarrasArrecadacaoSintetica(segmento: segmento));

        Assert.True(r.IsValid);
        Assert.Equal(SegmentoArrecadacao.Desconhecido, r.Segmento);
        Assert.True(TemErro(r, BoletoValidacaoErrorCodes.SegmentoDesconhecido, bloqueante: false));
    }

    [Theory]
    [InlineData('1', SegmentoArrecadacao.Prefeitura)]
    [InlineData('2', SegmentoArrecadacao.Saneamento)]
    [InlineData('3', SegmentoArrecadacao.EnergiaEletricaGas)]
    [InlineData('4', SegmentoArrecadacao.Telecomunicacoes)]
    [InlineData('5', SegmentoArrecadacao.OrgaosGovernamentais)]
    [InlineData('6', SegmentoArrecadacao.Outros)]
    [InlineData('7', SegmentoArrecadacao.TaxasTransito)]
    [InlineData('9', SegmentoArrecadacao.Outros)]
    public void Arrecadacao_MapaDeSegmentos_ComoNoLegado(char segmento, SegmentoArrecadacao esperado)
        => Assert.Equal(esperado, Validador().Validar(BoletoVetores.BarrasArrecadacaoSintetica(segmento: segmento)).Segmento);

    [Fact]
    public void Arrecadacao_SemDataNoCampoLivre_UsaDMaisUm()
    {
        // Campo livre sem AAAAMMDD válido nas posições 20-27.
        var barras = BoletoVetores.BarrasArrecadacaoSintetica(dados: "30119999999999999999999999999");
        var r = Validador(2026, 11, 9).Validar(barras);

        Assert.True(r.IsValid);
        Assert.True(r.VencimentoEstimado);
        Assert.Equal(OrigemVencimento.RegraDMaisUm, r.OrigemVencimento);
        Assert.Equal(new DateOnly(2026, 11, 10), r.DataVencimento);
    }

    [Fact]
    public void Arrecadacao_InterpretacaoDoCampoLivreDesligada_UsaDMaisUm()
    {
        var r = Validador(2026, 11, 9).Validar(
            BoletoVetores.BarrasArrecadacao[0],
            new BoletoValidacaoOptions { InterpretarDataDoCampoLivreDeArrecadacao = false });

        Assert.Equal(OrigemVencimento.RegraDMaisUm, r.OrigemVencimento);
        Assert.True(r.VencimentoEstimado);
    }

    [Fact]
    public void Arrecadacao_Vencida_BloqueiaPorPadrao()
    {
        // O código real vence 16/11/2026; em 17/11 está vencido.
        var r = Validador(2026, 11, 17).Validar(BoletoVetores.BarrasArrecadacao[0]);

        Assert.False(r.IsValid);
        Assert.True(r.Vencido);
    }

    // ---- robustez: entrada, janela e fuso -----------------------------------------------

    [Fact]
    public void EntradaGigante_Eh_RejeitadaSemProcessar()
    {
        var r = Validador().Validar(new string('1', 1_000_000));

        Assert.False(r.IsValid);
        Assert.Equal(BoletoValidacaoErrorCodes.TamanhoInvalido, Assert.Single(r.Errors).Code);
        Assert.Equal(string.Empty, r.CodigoNormalizado);
    }

    [Fact]
    public void LinhaFormatadaComSeparadores_NaoPassaDoLimiteDeEntrada()
    {
        var formatada = "0339 9.50461 76900.419847 44270.601022 4 1629 0000711270";

        Assert.True(formatada.Length < CodigoBoletoNormalizer.TamanhoMaximoEntrada);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void JanelaFuturaForaDoIntervalo_Eh_Ajustada_SemLancar(int janela)
    {
        var r = Validador().Validar(BoletoVetores.LinhasBancarias[0], new BoletoValidacaoOptions { JanelaFuturaFatorDias = janela });

        Assert.True(r.IsValid);
        Assert.Equal(CicloFatorVencimento.Novo, r.CicloFator); // negativo volta ao padrão; enorme é limitado a 36500
    }

    [Fact]
    public void FusoHorario_Configurado_DefineAData_EmHostUtc()
    {
        // 17/11 02:00 UTC = 16/11 23:00 em São Paulo. O boleto real vence em 16/11/2026 (arrecadação).
        var validador = new BoletoValidator(new RelogioFixo(2026, 11, 17, hora: 2), Microsoft.Extensions.Options.Options.Create(new BoletoValidacaoOptions()));

        var semFuso = validador.Validar(BoletoVetores.BarrasArrecadacao[0]);
        var comFuso = validador.Validar(BoletoVetores.BarrasArrecadacao[0], new BoletoValidacaoOptions { FusoHorarioId = "America/Sao_Paulo" });

        Assert.True(semFuso.Vencido);   // o relógio "local" do host está em UTC e já virou o dia
        Assert.False(comFuso.Vencido);  // no fuso do negócio ainda é dia 16
        Assert.True(comFuso.IsValid);
    }

    [Fact]
    public void FusoHorario_Inexistente_Lanca()
        => Assert.Throws<TimeZoneNotFoundException>(() =>
            Validador().Validar(BoletoVetores.LinhasBancarias[0], new BoletoValidacaoOptions { FusoHorarioId = "Marte/Olympus" }));

    // ---- injeção de dependência ---------------------------------------------------------

    [Fact]
    public void AddDietcodeBoletos_RegistraValidadorEAplicaOpcoes()
    {
        var services = new ServiceCollection();
        services.AddDietcodeBoletos(o => o.BloquearVencido = false);
        services.AddSingleton<TimeProvider>(new RelogioFixo(2026, 11, 20));

        using var provider = services.BuildServiceProvider();
        var validador = provider.GetRequiredService<IBoletoValidator>();

        var r = validador.Validar(BoletoVetores.LinhasBancarias[1]);

        Assert.True(r.Vencido);
        Assert.True(r.IsValid); // BloquearVencido = false veio das opções registradas
    }
}
