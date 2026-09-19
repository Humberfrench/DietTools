using Dietcode.Core.Lib.Pagging;
using Xunit;

namespace Dietcode.UnitTests.CoreLib.Pagging;

public class PaggingTests
{
    [Fact]
    public void ToPaged_ComListaEmMemoria_PaginaCorretamente()
    {
        var items = Enumerable.Range(1, 25).ToList();
        var parametro = new PageParameter { PageNumber = 2, PageSize = 10 };

        var pagina = items.ToPaged(parametro);

        Assert.Equal(25, pagina.TotalItems);
        Assert.Equal(10, pagina.Items.Count);
        Assert.Equal(2, pagina.PageNumber);
        Assert.Equal(3, pagina.TotalPages);
        Assert.True(pagina.HasPrevious);
        Assert.True(pagina.HasNext);
        Assert.Equal(1, pagina.PreviousPage);
        Assert.Equal(3, pagina.NextPage);
        Assert.Equal(11, pagina.Items[0]);
    }

    [Fact]
    public void ToPaged_NaUltimaPagina_NaoTemProxima()
    {
        var items = Enumerable.Range(1, 25).ToList();
        var parametro = new PageParameter { PageNumber = 3, PageSize = 10 };

        var pagina = items.ToPaged(parametro);

        Assert.Equal(5, pagina.Items.Count);
        Assert.False(pagina.HasNext);
        Assert.Null(pagina.NextPage);
    }

    [Fact]
    public void ToPaged_ComQueryable_PaginaCorretamente()
    {
        var items = Enumerable.Range(1, 25).AsQueryable();
        var parametro = new PageParameter { PageNumber = 1, PageSize = 5 };

        var pagina = items.ToPaged(parametro);

        Assert.Equal(25, pagina.TotalItems);
        Assert.Equal(5, pagina.Items.Count);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, pagina.Items);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(1000, PageParameter.MaxPageSize)]
    public void PageParameter_PageSize_EhSempreClampeado(int valorInformado, int esperado)
    {
        var parametro = new PageParameter { PageSize = valorInformado };

        Assert.Equal(esperado, parametro.PageSize);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-3, 1)]
    [InlineData(5, 5)]
    public void PageParameter_PageNumber_NuncaFicaMenorQueUm(int valorInformado, int esperado)
    {
        var parametro = new PageParameter { PageNumber = valorInformado };

        Assert.Equal(esperado, parametro.PageNumber);
    }
}
