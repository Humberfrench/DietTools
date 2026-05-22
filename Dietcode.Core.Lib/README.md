# Dietcode.Core.Lib

Biblioteca de utilitarios para aplicacoes .NET, reunindo extensoes, formatadores, validadores, helpers JSON, criptografia, mascaramento de dados, paginacao, localizacao, validacao de senha e chamadas REST simples.

## Instalacao

```bash
dotnet add package Dietcode.Core.Lib --version 10.6.0
```

## Funcionalidades

- Extensoes para string, numeros, datas, JSON e enums.
- Validacao e formatacao de documentos brasileiros, como CPF e CNPJ.
- Validadores de telefone, cartao, boletos e dados comuns.
- Criptografia AES.
- Mascaramento de dados sensiveis.
- Conversores JSON flexiveis.
- Analise de forca de senha.
- Paginacao.
- Localizacao simples por dicionario.
- Helper REST para chamadas HTTP com retorno padronizado.

## Extensoes comuns

```csharp
using Dietcode.Core.Lib;

var nome = "Maria Silva".GetFirstName();
var documento = "12345678901".ToCpf();
var somenteNumeros = "(11) 99999-9999".OnlyNumbers();
var temValor = "texto".HasValue();
var json = new { Id = 1, Nome = "Maria" }.ToJson();
```

Tambem existem extensoes para:

- `ToSnakeCase()`
- `ToCamelCase()`
- `ToKebabCase()`
- `RemoveAccents()`
- `IsValidEmail()`
- `ToMoeda()`
- `ToSimNao()`
- `ToPhoneFormated()`

## Datas

```csharp
using Dietcode.Core.Lib;

var data = DateTime.UtcNow;

var dataFormatada = data.ToDateFormatted();
var dataHora = data.ToDateTimeFormatted();
var dataHoraComSegundos = data.ToDateTimeWithSecondsFormatted();
var proximoDiaUtil = data.ProximoDiaUtil();
```

Tambem ha suporte a `DateOnly`:

```csharp
var hoje = DateOnly.FromDateTime(DateTime.Today);

var texto = hoje.ToDateFormatted();
var julian = hoje.ToJulianDateString();
var diaUtil = hoje.IsDiaUtil();
```

## Documentos e validacoes

```csharp
using Dietcode.Core.Lib;

var cpfValido = Validacao.IsCpf("12345678909");
var cnpjValido = Validacao.IsCnpj("12345678000195");

var cpfFormatado = "12345678909".ToCpf();
var cnpjFormatado = "12345678000195".ToCnpj();
var documento = "12345678909".FormatoCpfouCnpj();
```

## JSON

O pacote usa `System.Text.Json` e possui opcoes padrao em `JsonOptionsFactory`.

```csharp
using Dietcode.Core.Lib;
using Dietcode.Core.Lib.JsonConverting;

var options = JsonOptionsFactory.CreateDefault();

var json = Extensions.SerializeObject(new { Id = 1, Nome = "Maria" }, options);
var objeto = json.ToObject<MinhaClasse>(options);
```

As opcoes padrao incluem:

- nomes case-insensitive;
- politica camelCase;
- `ReferenceHandler.IgnoreCycles`;
- leitura de numeros como string;
- conversores flexiveis para valores e strings;
- ignorar propriedades nulas ao serializar.

## Senhas

```csharp
using Dietcode.Core.Lib.Passwords;

var result = "Senha@123".AsSpan().AnalyzePassword();

if (result.MeetsMinimumRules)
{
    // Senha atende as regras minimas.
}
```

A analise considera:

- tamanho minimo;
- letras maiusculas;
- letras minusculas;
- numeros;
- simbolos;
- espacos em branco;
- caracteres fora de ASCII;
- entropia estimada;
- nivel de forca.

## REST

A pasta `Rest` oferece um helper estatico para chamadas HTTP simples.

Componentes principais:

- `HttpService`: executa chamadas HTTP.
- `ApiResult<TResponse>`: resposta padronizada.
- `EnumApiRest`: tipo de autenticacao.

### Tipos de autenticacao

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

O helper tambem oferece:

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

`Content` guarda o body bruto da resposta, inclusive em caso de erro. Isso ajuda em diagnostico quando a API externa retorna texto, HTML, JSON inesperado ou mensagens fora do contrato.

### Observacoes sobre o helper REST

- `TResponse` precisa ser uma classe com construtor vazio (`where TResponse : class, new()`).
- Respostas primitivas como `bool`, `int` e `decimal` nao sao suportadas diretamente por `ApiResult<TResponse>` na versao atual.
- Cada chamada cria um `HttpClient` internamente.
- Falhas de transporte, como timeout, DNS ou conexao recusada, podem subir como excecao para o chamador.
- O body so e desserializado quando a resposta parece JSON.

## Localizacao

```csharp
using Dietcode.Core.Lib.Langs;

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Localization>();

Localization.AddDictionary("pt-BR", new Dictionary<string, string>
{
    ["Hello World!"] = "Ola Mundo!"
});
```

Uso em classe ou controller:

```csharp
public sealed class HomeController
{
    private readonly Localization _localization;

    public HomeController(Localization localization)
    {
        _localization = localization;
    }

    public string Index()
    {
        return _localization["Hello World!"];
    }
}
```

## Mascaramento de dados sensiveis

```csharp
using Dietcode.Core.Lib.Masking;

var masked = SensitiveDataMasker.Mask(new
{
    Email = "user@exemplo.com",
    Password = "123456",
    Token = "abc"
});
```

## Licenca

MIT
