# Dietcode.Core.Lib.Rest

Helper estático para chamadas HTTP simples, com retorno padronizado. Extraído de `Dietcode.Core.Lib` para isolar quem só precisa de utilitários gerais (validação, extensões etc.) de uma dependência de REST.

## Instalação

```bash
dotnet add package Dietcode.Core.Lib.Rest
```

## Funcionalidades

- `HttpService`: executa chamadas HTTP (GET, POST, PUT, PATCH, DELETE).
- `ApiResult<TResponse>`: resposta padronizada.
- `EnumApiRest`: tipo de autenticação.

### Tipos de autenticação

```csharp
EnumApiRest.None
EnumApiRest.Basic
EnumApiRest.Bearer
EnumApiRest.XApiKey
```

### GET

```csharp
using Dietcode.Core.Lib.Rest;

ApiResult<UserResponse> result = await HttpService.Get<UserResponse>(
    url: "https://api.exemplo.com/users/1",
    enumApiRest: EnumApiRest.Bearer,
    token: accessToken,
    cancellationToken: cancellationToken);

if (result.IsSuccess)
{
    var user = result.Data;
}
else
{
    var erro = result.Error;
    var body = result.Content;
}
```

### GET com query string

```csharp
var query = new Dictionary<string, object>
{
    ["page"] = 1,
    ["pageSize"] = 20,
    ["active"] = true
};

ApiResult<UserListResponse> result = await HttpService.Get<UserListResponse>(
    url: "https://api.exemplo.com/users",
    querystringParameter: query,
    enumApiRest: EnumApiRest.XApiKey,
    token: apiKey,
    cancellationToken: cancellationToken);
```

### POST com JSON

```csharp
var payload = new CreateUserRequest
{
    Name = "Maria",
    Email = "maria@exemplo.com"
};

ApiResult<CreateUserResponse> result =
    await HttpService.Post<CreateUserRequest, CreateUserResponse>(
        url: "https://api.exemplo.com/users",
        payload: payload,
        enumApiRest: EnumApiRest.Bearer,
        token: accessToken,
        cancellationToken: cancellationToken);
```

### PUT, PATCH e DELETE

O helper também oferece:

- `Put<TRequest, TResponse>()`
- `Patch<TRequest, TResponse>()`
- `Delete<TRequest, TResponse>()`
- `Delete<TResponse>()`

### ApiResult

```csharp
public class ApiResult<TResponse> where TResponse : class, new()
{
    public TResponse Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public DateTime TimeStamp { get; set; }
    public bool IsSuccess { get; set; }
    public string Content { get; set; }
    public string? ContentType { get; set; }
    public long? ContentLength { get; set; }
    public string Error { get; set; }
}
```

`IsSuccess` reflete `HttpResponseMessage.IsSuccessStatusCode`, ou seja, status HTTP `2xx`.

`Content` guarda o body bruto da resposta, inclusive em caso de erro. Isso ajuda em diagnóstico quando a API externa retorna texto, HTML, JSON inesperado ou mensagens fora do contrato.

### Observações sobre o helper REST

- `TResponse` precisa ser uma classe com construtor vazio (`where TResponse : class, new()`).
- Respostas primitivas como `bool`, `int` e `decimal` não são suportadas diretamente por `ApiResult<TResponse>` na versão atual.
- Cada chamada cria um `HttpClient` internamente.
- Falhas de transporte, como timeout, DNS ou conexão recusada, podem subir como exceção para o chamador.
- O body só é desserializado quando a resposta parece JSON.

## Pacotes relacionados

- `Dietcode.Core.Lib.Helpers`: fornece `JsonOptionsFactory` e os conversores JSON flexíveis usados internamente por `HttpService`. É um projeto auxiliar interno, sem `PackageId` próprio — seu binário é embutido direto neste pacote (e no `Dietcode.Core.Lib`), não é instalado separadamente.
- `Dietcode.Core.Lib`: versão anterior deste helper morava lá; a documentação de REST foi movida para cá.

## Licença

MIT
