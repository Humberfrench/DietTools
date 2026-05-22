# Dietcode.Api.Core.Results

Biblioteca de resultados padronizados para aplicacoes .NET e APIs ASP.NET Core. O pacote fornece objetos de retorno simples para representar sucesso, erro, conteudo, status HTTP e validacoes de forma consistente entre services, handlers e controllers.

Este pacote e usado diretamente por `Dietcode.Api.Core`.

## Instalacao

```bash
dotnet add package Dietcode.Api.Core.Results --version 10.5.0
```

## Objetivo

O objetivo e evitar que a camada de aplicacao dependa diretamente de `IActionResult`, `ControllerBase` ou detalhes de HTTP.

Em vez disso, services retornam `MethodResult`:

```csharp
using Dietcode.Api.Core.Results;

public sealed class UserService : AppServiceBase
{
    public async Task<MethodResult<UserDto>> GetAsync(int id, CancellationToken ct)
    {
        var user = await _repository.GetAsync(id, ct);

        if (user is null)
            return NotFound<UserDto>("Usuario nao encontrado.");

        return Ok(user);
    }
}
```

O controller, quando usa `Dietcode.Api.Core`, fica responsavel apenas por chamar `Completed(result)`.

## Funcionalidades

- `MethodResult`: resultado base com `ResultStatusCode`.
- `MethodResult<TContent>`: resultado com conteudo.
- Resultados de sucesso: `OkResult`, `CreatedResult`, `AcceptedResult`, `NoContentResult`.
- Resultados de erro: `BadRequestResult`, `NotFoundResult`, `ConflictResult`, `UnauthorizedResult`, `ForbiddenResult`, `UnprocessableEntityResult`, `TooManyRequestsResult`, entre outros.
- `ErrorResult`: resultado com lista de erros.
- `ErrorValidation`: objeto simples com `Code` e `Message`.
- `ErrorBuilder`: cria erros a partir de enums e `ResourceManager`.
- `AppServiceBase`: classe base com metodos de fabrica para resultados comuns.
- Interfaces `IContentResult` e `IErrorResult` para identificar resultados com conteudo ou erros.

## Modelo principal

```csharp
public class MethodResult
{
    public bool IsError => (int)Status >= 400;
    public ResultStatusCode Status { get; set; }
}

public class MethodResult<TContent> : MethodResult
{
    public TContent Content { get; set; }
}
```

## Status suportados

`ResultStatusCode` possui os principais codigos usados pela biblioteca:

- `OK` = 200
- `Created` = 201
- `Accepted` = 202
- `NoContent` = 204
- `BadRequest` = 400
- `Unauthorized` = 401
- `Forbidden` = 403
- `NotFound` = 404
- `NotAcceptable` = 406
- `TimeOut` = 408
- `Conflict` = 409
- `UnprocessableEntity` = 422
- `TooManyRequests` = 429
- `ClientClosedRequest` = 499
- `InternalServerError` = 500
- `ServiceUnavailable` = 503
- `InternalPersonalError` = 600
- `InternalPersonalWarning` = 601

## AppServiceBase

`AppServiceBase` facilita a criacao de resultados dentro da camada de aplicacao.

```csharp
using Dietcode.Api.Core.Results;

public sealed class UserService : AppServiceBase
{
    public MethodResult<UserDto> Get(int id)
    {
        if (id <= 0)
            return BadRequest<UserDto>("Id invalido.", new UserDto());

        var user = new UserDto { Id = id, Name = "Maria" };
        return Ok(user);
    }
}
```

Metodos disponiveis:

- `Ok()`
- `Ok<TContent>(TContent content)`
- `Created()`
- `Created<TContent>(TContent content, object id)`
- `Accepted()`
- `Accepted<TContent>(TContent content, object id)`
- `BadRequest(...)`
- `NotFound(...)`
- `TimeOut(...)`
- `Conflict(...)`
- `NotAcceptable(...)`
- `Forbidden()`
- `InternalServerError(Exception ex)`

## Erros

Um erro simples pode ser criado com `ErrorValidation`:

```csharp
var error = new ErrorValidation("USR001", "Usuario nao encontrado.");
return new NotFoundResult(error);
```

Varios erros podem ser retornados juntos:

```csharp
var errors = new[]
{
    new ErrorValidation("USR001", "Nome obrigatorio."),
    new ErrorValidation("USR002", "Email invalido.")
};

return new BadRequestResult(errors);
```

Quando combinado com `Dietcode.Api.Core`, multiplos erros sao convertidos para `ValidationProblemDetails`.

## ErrorBuilder

`ErrorBuilder` permite criar erros a partir de enums.

```csharp
public enum UserErrors
{
    UserNotFound = 1,
    InvalidEmail = 2
}

var builder = new ErrorBuilder();
var error = builder.GetError(UserErrors.UserNotFound);
```

O codigo numerico do enum e convertido para string com tres digitos, por exemplo `001`.

## Uso com Dietcode.Api.Core

Fluxo completo:

```csharp
public sealed class UserService : AppServiceBase
{
    public MethodResult<UserDto> Get(int id)
    {
        if (id <= 0)
            return BadRequest<UserDto>("Id invalido.", new UserDto());

        return Ok(new UserDto { Id = id });
    }
}
```

```csharp
[HttpGet("{id:int}")]
public IActionResult Get(int id)
{
    var result = _service.Get(id);
    return Completed<UserDto>(result);
}
```

## Pacotes relacionados

- `Dietcode.Api.Core`: conversao dos resultados para respostas HTTP em ASP.NET Core.

## Licenca

MIT
