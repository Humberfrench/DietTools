using Dietcode.Api.Core.Results;
using Xunit;

namespace Dietcode.UnitTests.ApiCoreResults;

public class MethodResultTests
{
    [Theory]
    [InlineData(ResultStatusCode.OK, false)]
    [InlineData(ResultStatusCode.Created, false)]
    [InlineData(ResultStatusCode.BadRequest, true)]
    [InlineData(ResultStatusCode.NotFound, true)]
    [InlineData(ResultStatusCode.InternalServerError, true)]
    public void IsError_RefleteOStatusCode(ResultStatusCode status, bool esperado)
    {
        var result = new MethodResult(status);

        Assert.Equal(esperado, result.IsError);
    }

    [Fact]
    public void MethodResultGenerico_ArmazenaConteudo()
    {
        var result = new MethodResult<string>("conteudo", ResultStatusCode.OK);

        Assert.Equal("conteudo", result.Content);
        Assert.False(result.IsError);
    }

    [Fact]
    public void OkResult_UsaStatusCodeOk()
    {
        var result = new OkResult<int>(42);

        Assert.Equal(ResultStatusCode.OK, result.Status);
        Assert.Equal(42, result.Content);
        Assert.False(result.IsError);
    }

    [Fact]
    public void BadRequestResult_CarregaErro()
    {
        var error = new ErrorValidation("001", "Campo obrigatorio.");
        var result = new BadRequestResult(error);

        Assert.Equal(ResultStatusCode.BadRequest, result.Status);
        Assert.True(result.IsError);
        Assert.Single(result.Errors);
        Assert.Equal("Campo obrigatorio.", result.Errors.First().Message);
    }

    [Fact]
    public void BadRequestResultGenerico_CarregaConteudoEErro()
    {
        var error = new ErrorValidation("001", "Id invalido.");
        var result = new BadRequestResult<int>(0, error);

        Assert.Equal(0, result.Content);
        Assert.Equal(ResultStatusCode.BadRequest, result.Status);
        Assert.Single(result.Errors);
    }
}
