using Dietcode.Api.Core.Results;
using Dietcode.Api.Core.Results.Interfaces;
using Xunit;

namespace Dietcode.UnitTests.ApiCoreResults;

public class AppServiceBaseTests
{
    private sealed class TestAppService : AppServiceBase
    {
        // Propagate() é protected em AppServiceBase; exposto aqui só para o teste.
        public MethodResult<TContent> TestPropagate<TContent>(MethodResult error, TContent content = default!)
            => Propagate(error, content);
    }

    [Fact]
    public void Ok_RetornaStatusCodeOk()
    {
        var service = new TestAppService();

        var result = service.Ok(new { Id = 1 });

        Assert.Equal(ResultStatusCode.OK, result.Status);
        Assert.False(result.IsError);
    }

    [Fact]
    public void BadRequest_ComMensagem_RetornaErro()
    {
        var service = new TestAppService();

        var result = service.BadRequest<bool>("Id invalido.", false);

        Assert.Equal(ResultStatusCode.BadRequest, result.Status);
        Assert.True(result.IsError);
        Assert.False(result.Content);
        Assert.Equal("Id invalido.", result.Errors.First().Message);
    }

    [Fact]
    public void NotFound_ComMensagem_RetornaErro()
    {
        var service = new TestAppService();

        var result = service.NotFound("Usuario nao encontrado.");

        Assert.Equal(ResultStatusCode.NotFound, result.Status);
        Assert.True(result.IsError);
    }

    [Fact]
    public void Propagate_ComErroDeMethodResultInterno_PreservaStatusEErros()
    {
        var service = new TestAppService();

        MethodResult erroInterno = service.NotFound("Operacao nao encontrada.");

        MethodResult<bool> propagado = service.TestPropagate<bool>(erroInterno, false);

        Assert.Equal(ResultStatusCode.NotFound, propagado.Status);
        Assert.True(propagado.IsError);
        Assert.False(propagado.Content);

        var errorResult = Assert.IsAssignableFrom<IErrorResult>(propagado);
        Assert.Equal("Operacao nao encontrada.", errorResult.Errors.First().Message);
    }

    [Fact]
    public void Propagate_ComResultadoSemErro_PreservaApenasStatus()
    {
        var service = new TestAppService();

        MethodResult sucesso = service.Ok();

        MethodResult<int> propagado = service.TestPropagate(sucesso, 10);

        Assert.Equal(ResultStatusCode.OK, propagado.Status);
        Assert.Equal(10, propagado.Content);
    }
}
