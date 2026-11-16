using Dietcode.Core.Lib.Boletos;
using Xunit;

namespace Dietcode.UnitTests.CoreLibBoletos;

public class HelpersBoletoTests
{
    [Fact]
    public void Modulo10_CalculaComPesos2e1()
    {
        // Campo 1 da linha real 341...: "341910905" -> DV 7 (visto na linha 34191090574...).
        Assert.Equal(7, DigitoVerificadorHelper.Modulo10("341910905"));
        Assert.Equal(0, DigitoVerificadorHelper.Modulo10("0"));
    }

    [Theory]
    [InlineData("12a4")]
    [InlineData("12 4")]
    public void Modulos_RejeitamEntradaNaoNumerica(string entrada)
    {
        Assert.Throws<ArgumentException>(() => DigitoVerificadorHelper.Modulo10(entrada));
        Assert.Throws<ArgumentException>(() => DigitoVerificadorHelper.Modulo11Bancario(entrada));
        Assert.Throws<ArgumentException>(() => DigitoVerificadorHelper.Modulo11Arrecadacao(entrada));
    }

    [Fact]
    public void Modulo11Bancario_RestoQueDa0_10Ou11_ViraUm()
    {
        // "0" -> soma 0 -> 11 - 0 = 11 -> 1 ; "5" -> soma 10 -> resto 10 -> dv 1.
        Assert.Equal(1, DigitoVerificadorHelper.Modulo11Bancario("0"));
        Assert.Equal(1, DigitoVerificadorHelper.Modulo11Bancario("5"));
        // "1" -> soma 2 -> 11 - 2 = 9.
        Assert.Equal(9, DigitoVerificadorHelper.Modulo11Bancario("1"));
    }

    [Fact]
    public void Modulo11Arrecadacao_RestoZeroOuUm_ViraZero()
    {
        Assert.Equal(0, DigitoVerificadorHelper.Modulo11Arrecadacao("0"));   // soma 0, resto 0
        Assert.Equal(0, DigitoVerificadorHelper.Modulo11Arrecadacao("6"));   // soma 12, resto 1
        Assert.Equal(9, DigitoVerificadorHelper.Modulo11Arrecadacao("1"));   // soma 2, 11 - 2
        Assert.Equal(1, DigitoVerificadorHelper.Modulo11Arrecadacao("5"));   // soma 10, resto 10 -> 1
    }

    [Theory]
    [InlineData('6', true)]
    [InlineData('7', true)]
    [InlineData('8', true)]
    [InlineData('9', true)]
    [InlineData('5', false)]
    [InlineData('0', false)]
    public void DigitoArrecadacao_SoExisteParaIndicador6a9(char indicador, bool existe)
        => Assert.Equal(existe, DigitoVerificadorHelper.DigitoArrecadacao(indicador, "12345").HasValue);

    [Fact]
    public void DigitoArrecadacao_Indicador6e7UsamModulo10_8e9UsamModulo11()
    {
        const string corpo = "1234567890";
        Assert.Equal(DigitoVerificadorHelper.Modulo10(corpo), DigitoVerificadorHelper.DigitoArrecadacao('6', corpo));
        Assert.Equal(DigitoVerificadorHelper.Modulo10(corpo), DigitoVerificadorHelper.DigitoArrecadacao('7', corpo));
        Assert.Equal(DigitoVerificadorHelper.Modulo11Arrecadacao(corpo), DigitoVerificadorHelper.DigitoArrecadacao('8', corpo));
        Assert.Equal(DigitoVerificadorHelper.Modulo11Arrecadacao(corpo), DigitoVerificadorHelper.DigitoArrecadacao('9', corpo));
    }

    // ---- fator de vencimento ------------------------------------------------------------

    private static readonly DateOnly Hoje = new(2026, 11, 16);

    [Fact]
    public void Fator9999_NoCicloOriginal_Eh21de02de2025()
    {
        var r = FatorVencimentoHelper.CalcularVencimento(9999, Hoje, 5500);

        Assert.Equal(new DateOnly(2025, 2, 21), r.Data);
        Assert.Equal(CicloFatorVencimento.Original, r.Ciclo);
    }

    [Theory]
    [InlineData(1000, 2025, 2, 22)]
    [InlineData(1001, 2025, 2, 23)]
    [InlineData(1626, 2026, 11, 10)]
    public void FatorDoCicloNovo_ContaApartirDe22de02de2025(int fator, int ano, int mes, int dia)
    {
        var r = FatorVencimentoHelper.CalcularVencimento(fator, Hoje, 5500);

        Assert.Equal(new DateOnly(ano, mes, dia), r.Data);
        Assert.Equal(CicloFatorVencimento.Novo, r.Ciclo);
    }

    [Fact]
    public void Fator0000_NaoTemData_NemEhInvalido()
    {
        var r = FatorVencimentoHelper.CalcularVencimento(0, Hoje, 5500);

        Assert.Null(r.Data);
        Assert.False(r.FatorInvalido);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    public void FatorEntre0001e0999_Eh_Invalido(int fator)
    {
        var r = FatorVencimentoHelper.CalcularVencimento(fator, Hoje, 5500);

        Assert.Null(r.Data);
        Assert.True(r.FatorInvalido);
    }

    [Fact]
    public void JanelaFutura_DecideEntreCicloNovoEOriginal()
    {
        // Fator cuja data nova cai exatamente em hoje + 5500: ainda é o ciclo novo.
        var limite = Hoje.AddDays(5500);
        var fatorNoLimite = 1000 + limite.DayNumber - FatorVencimentoHelper.InicioCicloNovo.DayNumber;

        Assert.Equal(CicloFatorVencimento.Novo, FatorVencimentoHelper.CalcularVencimento(fatorNoLimite, Hoje, 5500).Ciclo);
        Assert.Equal(CicloFatorVencimento.Original, FatorVencimentoHelper.CalcularVencimento(fatorNoLimite + 1, Hoje, 5500).Ciclo);
    }

    [Theory]
    [InlineData(2026, 11, 14, 2026, 11, 16)] // sábado -> segunda
    [InlineData(2026, 11, 15, 2026, 11, 16)] // domingo -> segunda
    [InlineData(2026, 11, 16, 2026, 11, 16)] // segunda fica
    [InlineData(2026, 11, 13, 2026, 11, 13)] // sexta fica
    public void AjustarParaDiaUtil_LevaFimDeSemanaParaSegunda(int a, int m, int d, int ea, int em, int ed)
        => Assert.Equal(new DateOnly(ea, em, ed), FatorVencimentoHelper.AjustarParaDiaUtil(new DateOnly(a, m, d)));

    [Theory]
    [InlineData("20261116", true)]
    [InlineData("20260230", false)]
    [InlineData("20261301", false)]
    [InlineData("19991231", false)]
    [InlineData("00000000", false)]
    [InlineData("2026111", false)]
    public void LerDataAnoMesDia_AceitaSoDataValidaDoSeculo(string texto, bool valida)
        => Assert.Equal(valida, FatorVencimentoHelper.LerDataAnoMesDia(texto).HasValue);

    // ---- normalização e conversores -----------------------------------------------------

    [Theory]
    [InlineData("03399.50461 76900.419847 44270.601022 4 16290000711270", "03399504617690041984744270601022416290000711270")]
    [InlineData("  8160-0000000-1 \n", "816000000001")]
    public void Normalizar_RemoveSeparadoresEPreservaZeros(string entrada, string esperado)
    {
        Assert.Null(CodigoBoletoNormalizer.Normalizar(entrada, out var normalizado));
        Assert.Equal(esperado, normalizado);
    }

    [Theory]
    [InlineData(null, BoletoValidacaoErrorCodes.CodigoVazio)]
    [InlineData("", BoletoValidacaoErrorCodes.CodigoVazio)]
    [InlineData("  .-  ", BoletoValidacaoErrorCodes.CodigoVazio)]
    [InlineData("1234a678", BoletoValidacaoErrorCodes.CodigoInvalido)]
    [InlineData("1234/5678", BoletoValidacaoErrorCodes.CodigoInvalido)]
    [InlineData("١٢٣٤", BoletoValidacaoErrorCodes.CodigoInvalido)] // dígitos arábicos não valem
    public void Normalizar_RejeitaVazioELetrasESimbolos(string? entrada, string codigoErro)
        => Assert.Equal(codigoErro, CodigoBoletoNormalizer.Normalizar(entrada, out _)!.Code);

    [Fact]
    public void Bancario_RoundTrip_44_47_44()
    {
        foreach (var linha in BoletoVetores.LinhasBancarias)
        {
            var barras = ConversorBoletoBancario.ParaCodigoBarras(linha);

            Assert.Equal(44, barras.Length);
            Assert.Equal(linha, ConversorBoletoBancario.ParaLinhaDigitavel(barras));
        }
    }

    [Fact]
    public void Arrecadacao_RoundTrip_44_48_44()
    {
        foreach (var barras in BoletoVetores.BarrasArrecadacao)
        {
            var linha = ConversorBoletoArrecadacao.ParaLinhaDigitavel(barras);

            Assert.Equal(48, linha!.Length);
            Assert.Equal(barras, ConversorBoletoArrecadacao.ParaCodigoBarras(linha));
        }
    }

    [Fact]
    public void Conversores_ExigemTamanhoExato()
    {
        Assert.Throws<ArgumentException>(() => ConversorBoletoBancario.ParaLinhaDigitavel("123"));
        Assert.Throws<ArgumentException>(() => ConversorBoletoBancario.ParaCodigoBarras(new string('1', 46)));
        Assert.Throws<ArgumentException>(() => ConversorBoletoArrecadacao.ParaLinhaDigitavel("123"));
        Assert.Throws<ArgumentException>(() => ConversorBoletoArrecadacao.ParaCodigoBarras(new string('8', 47)));
    }
}
