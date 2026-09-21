# Dietcode.Core.Lib

Biblioteca de utilitarios para aplicacoes .NET, reunindo extensoes, formatadores, validadores, helpers JSON, criptografia, mascaramento de dados, paginacao, localizacao e validacao de senha.

## Instalacao

```bash
dotnet add package Dietcode.Core.Lib --version 10.10.0
```

## Funcionalidades

- Extensoes para string, numeros, datas, JSON e enums.
- Validacao e formatacao de documentos brasileiros, como CPF e CNPJ.
- Validadores de telefone, cartao, boletos e dados comuns.
- Mascaramento de dados sensiveis.
- Conversores JSON flexiveis.
- Analise de forca de senha.
- Paginacao.
- Localizacao simples por dicionario.

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
using Dietcode.Core.Lib.Helpers.JsonConverting;

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

## Pacotes relacionados

- `Dietcode.Core.Security`: criptografia AES (AES-GCM atual e AES-ECB legado). Esta funcionalidade morava neste pacote e foi movida para lá.
- `Dietcode.Core.Lib.Rest`: helper REST (`HttpService`, `ApiResult<T>`, `EnumApiRest`). Esta funcionalidade morava neste pacote e foi movida para lá.
- `Dietcode.Core.Lib.Helpers`: projeto auxiliar interno (sem `PackageId` próprio) com `JsonOptionsFactory` e os conversores JSON flexíveis; seu binário é embutido neste pacote.

## Licenca

MIT
