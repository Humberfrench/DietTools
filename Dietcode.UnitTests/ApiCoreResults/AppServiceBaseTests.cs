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

    [Fact]
    public void NoContent_RetornaStatus204SemErro()
    {
        var result = new TestAppService().NoContent();

        Assert.Equal(ResultStatusCode.NoContent, result.Status);
        Assert.False(result.IsError);
    }

    [Fact]
    public void ServiceUnavailable_Retorna503()
    {
        var result = new TestAppService().ServiceUnavailable();

        Assert.Equal(ResultStatusCode.ServiceUnavailable, result.Status);
        Assert.True(result.IsError);
    }

    [Fact]
    public void InternalPersonalError_Retorna600()
    {
        var result = new TestAppService().InternalPersonalError();

        Assert.Equal(ResultStatusCode.InternalPersonalError, result.Status);
    }

    [Fact]
    public void InternalPersonalWarning_Retorna601()
    {
        var result = new TestAppService().InternalPersonalWarning();

        Assert.Equal(ResultStatusCode.InternalPersonalWarning, result.Status);
    }

    [Fact]
    public void TooManyRequests_SemArgumentos_UsaMensagemPadrao()
    {
        var result = new TestAppService().TooManyRequests();

        Assert.Equal(ResultStatusCode.TooManyRequests, result.Status);
        Assert.True(result.IsError);
        Assert.Equal("Too Many Requests", result.Errors.First().Message);
    }

    [Fact]
    public void BadRequestProblem_ComMensagem_PreencheProblemDetails()
    {
        var result = new TestAppService().BadRequestProblem("Campo invalido.");

        Assert.Equal(ResultStatusCode.BadRequest, result.Status);
        Assert.Equal(400, result.Content.Status);
        Assert.Equal("Campo invalido.", result.Content.Detail);
        Assert.Equal("Campo invalido.", result.Errors.First().Message);
    }

    [Fact]
    public void ClientClosed_ComMensagem_Retorna499()
    {
        var result = new TestAppService().ClientClosed("Cancelado.");

        Assert.Equal(ResultStatusCode.ClientClosedRequest, result.Status);
        Assert.True(result.IsError);
        Assert.Equal("Cancelado.", result.Errors.First().Message);
    }

    [Fact]
    public void ClientClosed_Generico_PreservaConteudo()
    {
        var result = new TestAppService().ClientClosed(42, "Cancelado.");

        Assert.Equal(ResultStatusCode.ClientClosedRequest, result.Status);
        Assert.Equal(42, result.Content);
    }

    [Fact]
    public void Unauthorized_ComMensagem_Retorna401()
    {
        var result = new TestAppService().Unauthorized("Token expirado.");

        Assert.Equal(ResultStatusCode.Unauthorized, result.Status);
        Assert.True(result.IsError);
        Assert.Equal("Token expirado.", result.Errors.First().Message);
    }

    [Fact]
    public void Unauthorized_ComListaDeErros_PreservaTodos()
    {
        var erros = new[] { new ErrorValidation("A", "um"), new ErrorValidation("B", "dois") };

        var result = new TestAppService().Unauthorized(erros);

        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void UnprocessableEntity_ComMensagem_Retorna422()
    {
        var result = new TestAppService().UnprocessableEntity("Regra de negocio violada.");

        Assert.Equal(ResultStatusCode.UnprocessableEntity, result.Status);
        Assert.True(result.IsError);
        Assert.Equal("Regra de negocio violada.", result.Errors.First().Message);
    }

    [Fact]
    public void BadRequestProblem_ComListaDeErros_UsaMensagensNoDetail()
    {
        var erros = new[] { new ErrorValidation("A", "primeiro"), new ErrorValidation("B", "segundo") };

        var result = new TestAppService().BadRequestProblem(erros);

        Assert.Equal("primeiro-segundo", result.Content.Detail);
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void BadRequestProblem_ComEnumeravelDeUsoUnico_MantemErros()
    {
        static IEnumerable<ErrorValidation> UmaVez()
        {
            yield return new ErrorValidation("A", "primeiro");
            yield return new ErrorValidation("B", "segundo");
        }

        var result = new TestAppService().BadRequestProblem(UmaVez());

        Assert.Equal(2, result.Errors.Count());
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public void ClientClosed_Generico_ComErrorValidation_PreservaCodigo()
    {
        var result = new TestAppService().ClientClosed(7, new ErrorValidation("499", "Cancelado."));

        Assert.Equal(7, result.Content);
        Assert.Equal("499", result.Errors.First().Code);
    }

    [Fact]
    public void ClientClosed_Generico_ComListaDeErros_PreservaTodos()
    {
        var erros = new[] { new ErrorValidation("A", "um"), new ErrorValidation("B", "dois") };

        var result = new TestAppService().ClientClosed("x", erros);

        Assert.Equal(2, result.Errors.Count());
    }
}
