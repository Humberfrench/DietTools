using Dietcode.Core.Password.Hibp;
using Xunit;

namespace Dietcode.UnitTests.CorePassword.Hibp;

public class HibpRangeParserTests
{
    private readonly HibpRangeParser _parser = new();

    [Fact]
    public void FindOccurrences_SuffixEncontrado_RetornaCount()
    {
        var response = "ABCDEF1234567890ABCDEF1234567890ABCD:1\nFEDCBA0987654321FEDCBA0987654321FEDC:42";

        Assert.Equal(42, _parser.FindOccurrences(response, "FEDCBA0987654321FEDCBA0987654321FEDC"));
    }

    [Fact]
    public void FindOccurrences_SuffixNaoEncontrado_RetornaZero()
    {
        var response = "ABCDEF1234567890ABCDEF1234567890ABCD:1";

        Assert.Equal(0, _parser.FindOccurrences(response, "0000000000000000000000000000000000000"));
    }

    [Fact]
    public void FindOccurrences_CountUm_RetornaUm()
    {
        const string suffix = "AAAA1111AAAA1111AAAA1111AAAA1111AAAA1";
        var response = $"{suffix}:1";

        Assert.Equal(1, _parser.FindOccurrences(response, suffix));
    }

    [Fact]
    public void FindOccurrences_CountAlto_RetornaValorCompleto()
    {
        const string suffix = "AAAA1111AAAA1111AAAA1111AAAA1111AAAA1";
        var response = $"{suffix}:9876543";

        Assert.Equal(9876543, _parser.FindOccurrences(response, suffix));
    }

    [Fact]
    public void FindOccurrences_PaddingComCountZero_EhIgnorado()
    {
        const string suffix = "AAAA1111AAAA1111AAAA1111AAAA1111AAAA1";
        var response = $"{suffix}:0";

        Assert.Equal(0, _parser.FindOccurrences(response, suffix));
    }

    [Fact]
    public void FindOccurrences_ComCRLF_FuncionaIgual()
    {
        const string suffix = "BBBB2222BBBB2222BBBB2222BBBB2222BBBB2";
        var response = $"AAAA1111AAAA1111AAAA1111AAAA1111AAAA1:1\r\n{suffix}:5\r\n";

        Assert.Equal(5, _parser.FindOccurrences(response, suffix));
    }

    [Fact]
    public void FindOccurrences_ComLF_FuncionaIgual()
    {
        const string suffix = "BBBB2222BBBB2222BBBB2222BBBB2222BBBB2";
        var response = $"AAAA1111AAAA1111AAAA1111AAAA1111AAAA1:1\n{suffix}:5\n";

        Assert.Equal(5, _parser.FindOccurrences(response, suffix));
    }

    [Fact]
    public void FindOccurrences_RespostaVazia_RetornaZero()
    {
        Assert.Equal(0, _parser.FindOccurrences("", "AAAA1111AAAA1111AAAA1111AAAA1111AAAA1"));
    }

    [Fact]
    public void FindOccurrences_LinhaInvalida_EhIgnoradaSemLancar()
    {
        const string suffix = "AAAA1111AAAA1111AAAA1111AAAA1111AAAA1";
        var response = $"linha sem separador\n{suffix}:3";

        Assert.Equal(3, _parser.FindOccurrences(response, suffix));
    }

    [Fact]
    public void FindOccurrences_ComparacaoIgnoraCase()
    {
        const string suffix = "AAAA1111AAAA1111AAAA1111AAAA1111AAAA1";
        var response = $"{suffix.ToLowerInvariant()}:7";

        Assert.Equal(7, _parser.FindOccurrences(response, suffix));
    }
}
