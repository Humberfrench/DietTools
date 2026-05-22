# Dietcode.Api.Core

Biblioteca de apoio para APIs ASP.NET Core. O pacote centraliza recursos comuns de infraestrutura HTTP, como conversao de resultados de aplicacao em respostas HTTP, logging de requests/responses e rate limiting simples por endpoint.

Este pacote depende de `Dietcode.Api.Core.Results` para o modelo padronizado de retorno.

## Instalacao

```bash
dotnet add package Dietcode.Api.Core --version 10.5.0
```

## Funcionalidades

- `ApiControllerBase`: controller base com helpers para converter `MethodResult` em `IActionResult`.
- Conversao automatica de erros para `ProblemDetails` ou `ValidationProblemDetails`.
- Tratamento padrao para respostas `OK`, `Created`, erros HTTP e requisicoes canceladas pelo cliente.
- Hook `BeforeReturn` para customizar auditoria, logs ou enriquecimento antes da resposta.
- Rate limit por atributo, baseado em IP e endpoint.
- Middleware de logging estruturado em JSON Lines (`.jsonl`), com mascara de dados sensiveis.
- Middleware legado de logging simples em texto.

## Fluxo recomendado

1. A camada de aplicacao retorna `MethodResult` ou `MethodResult<T>`.
2. O controller herda de `ApiControllerBase`.
3. A action chama `Completed(result)`.
4. A biblioteca converte o resultado para HTTP.

```csharp
using Dietcode.Api.Core;
using Dietcode.Api.Core.Results;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        MethodResult<UserDto> result = await _service.GetAsync(id, ct);
        return Completed<UserDto>(result);
    }
}
```

## ApiControllerBase

`ApiControllerBase` e o ponto principal de integracao com ASP.NET Core.

Ele oferece:

- `Completed(MethodResult result)`
- `Completed<TContent>(MethodResult<TContent> result)`
- `Canceled()`
- `Canceled<TContent>(TContent content)`
- `BeforeReturn(MethodResult result)`

Exemplo de customizacao:

```csharp
protected override MethodResult BeforeReturn(MethodResult result)
{
    // Use este ponto para auditoria, logs ou enriquecimento de erros.
    return result;
}
```

## Respostas de erro

Quando o resultado possui status HTTP `400` ou superior, o controller gera uma resposta padronizada.

- Um erro: `ProblemDetails`.
- Mais de um erro: `ValidationProblemDetails`.
- As respostas incluem `traceId`, `timestamp`, `status`, `title`, `type`, `detail` e `instance`.

## Created

Quando um `CreatedResult<T>` e retornado, o controller usa `CreatedAtAction` apontando para uma action chamada `Get` no mesmo controller, usando o identificador informado no resultado.

```csharp
public sealed class UserService : AppServiceBase
{
    public MethodResult<UserDto> Create(UserDto user)
    {
        return Created(user, user.Id);
    }
}
```

Observacao: se o controller nao tiver uma action `Get` compativel, prefira ajustar o fluxo antes de usar `CreatedResult<T>`.

## Logging estruturado

O middleware `ApiLoggingMiddleware` registra request e response em JSON Lines.

### appsettings.json

```json
{
  "ApiLogging": {
    "Directory": "logs",
    "Enabled": true
  }
}
```

### Program.cs

```csharp
using Dietcode.Api.Core.Extenders;

builder.Services.AddApiLogging(builder.Configuration);

var app = builder.Build();

app.UseApiLogging();
```

### Exemplo de log

```json
{
  "timestamp": "2026-01-16T14:33:21+00:00",
  "method": "POST",
  "url": "/api/users",
  "statusCode": 201,
  "traceId": "0HMS...",
  "request": "{\"email\":\"user@email.com\",\"password\":\"***\"}",
  "response": "{\"id\":10}"
}
```

Campos sensiveis sao mascarados pelo componente `SensitiveDataMasker` de `Dietcode.Core.Lib`.

Campos comuns mascarados:

- `password`
- `senha`
- `token`
- `accessToken`
- `refreshToken`

## Logging simples em texto

Tambem existe o middleware `RequestResponseLoggingMiddleware`, que grava logs em texto.

```csharp
app.UseMiddleware<RequestResponseLoggingMiddleware>();
```

Use este middleware apenas quando o log simples em arquivo texto for suficiente. Para APIs modernas, prefira `UseApiLogging()`.

## Rate limiting

O rate limit e aplicado por atributo em uma action. A implementacao atual usa `IMemoryCache`, portanto o controle e local por instancia da API.

```csharp
using Dietcode.Api.Core.Attributes;

[HttpGet]
[RateLimit(limit: 10, seconds: 60)]
public IActionResult Get()
{
    if (CheckRateLimit(out var rateLimitResult))
        return rateLimitResult!;

    return Ok();
}
```

### Registro dos servicos

```csharp
using Dietcode.Api.Core.Middleware;

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRateLimiter, RateLimiter>();
```

O contador em memoria usa incremento atomico para evitar perda de contagem sob concorrencia dentro da mesma instancia.

Quando o limite e excedido:

- O status retornado e `429 Too Many Requests`.
- O header `Retry-After` e preenchido.
- O payload segue o padrao de erro da biblioteca.

## Pacotes relacionados

- `Dietcode.Api.Core.Results`: modelo de resultados e erros.
- `Dietcode.Core.Lib`: utilitarios usados pelo logging, incluindo mascara de dados sensiveis.

## Licenca

MIT
