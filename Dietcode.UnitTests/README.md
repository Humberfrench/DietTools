# Dietcode.UnitTests

Projeto de testes unitários (xUnit) cobrindo as bibliotecas do repositório que não dependem de banco de dados: `Dietcode.Core.Lib`, `Dietcode.Api.Core.Results`, `Dietcode.Core.Email` e `Dietcode.Core.Cep`.

Não é publicado como pacote NuGet (`IsPackable=false`).

## Por que xUnit

xUnit foi escolhido por ser o padrão de facto para projetos .NET modernos (net10.0), com sintaxe enxuta (`[Fact]`/`[Theory]`) e integração nativa com `dotnet test`. Não há mocking framework (Moq etc.) como dependência: os cenários cobertos usam fakes/stubs escritos à mão (um `HttpMessageHandler` falso para os providers de CEP, implementações simples de `ICepProvider` para o failover), o que manteve o projeto mais simples sem perder clareza.

## Como rodar

```bash
dotnet test Dietcode.UnitTests/Dietcode.UnitTests.csproj
```

## Estrutura

Uma pasta por biblioteca testada, com subpastas por área quando fizer sentido:

```
Dietcode.UnitTests/
├── CoreLib/                    (Dietcode.Core.Lib)
│   ├── Validation/              CPF/CNPJ (Validacao, Documento) e e-mail (IsValidEmail)
│   ├── Extensions/               extensões de string (HasValue, OnlyNumbers, ToSnakeCase, RemoveAccents...)
│   ├── Passwords/                 análise de força de senha
│   ├── Masking/                   SensitiveDataMasker
│   └── Pagging/                   ToPaged / PageParameter
├── ApiCoreResults/              (Dietcode.Api.Core.Results)
│   ├── MethodResultTests.cs       MethodResult/OkResult/BadRequestResult
│   └── AppServiceBaseTests.cs     Ok/BadRequest/NotFound/Propagate
├── Email/                       (Dietcode.Core.Email)
│   └── SmtpEmailSenderValidationTests.cs
├── Cep/                         (Dietcode.Core.Cep)
│   ├── TestSupport/               StubHttpMessageHandler (fake de HttpClient, sem rede)
│   ├── ViaCepProviderTests.cs
│   ├── BrasilApiProviderTests.cs
│   ├── ContingencyCepProviderTests.cs  failover primário/secundário, Contingencia
│   └── CepLookupResultTests.cs
```

## O que fica de fora (por enquanto)

- **Banco de dados** (`Dietcode.Database*`): fora de escopo por pedido explícito.
- **`Dietcode.Api.Core`** (ASP.NET Core: `ApiControllerBase`, rate limiting, logging middleware): exigiria um host de testes ASP.NET Core (`ControllerContext`/`HttpContext` fake); não coberto nesta rodada.
- **Extensões que não valeu a pena testar isoladamente**: conversores span-based (`ToCpfSpan`, `ToSnakeCaseSpan` etc.) e helpers triviais de formatação (`ToMoeda`) foram deixados de fora — a cobertura já existe para a contraparte não-span quando a lógica é a mesma.
- **`HttpService`** (helper REST estático de `Dietcode.Core.Lib`): cria um `HttpClient` internamente (`new HttpClient()`), sem ponto de injeção — não dá para interceptar a chamada sem bater em rede real. Testável apenas com uma dependência externa ou um refactor (fora de escopo aqui).
- **Envio de e-mail bem-sucedido** (`SmtpEmailSender.SendAsync` com SMTP real): só a camada de validação (que roda antes de qualquer conexão) é coberta; testar o envio de fato exigiria um servidor SMTP real ou fake.

## Licença

MIT
